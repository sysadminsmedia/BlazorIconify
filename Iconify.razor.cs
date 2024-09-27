using System.Text;
using System.Text.Encodings.Web;
using System.Xml;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using SysAdminsMedia.BlazorIconify.Extensions;

namespace SysAdminsMedia.BlazorIconify;

public partial class Iconify : ComponentBase
{
    private const string API = "https://api.iconify.design/";
    private const string ErrorIcon = "ic:baseline-do-not-disturb";

    private string _svg = string.Empty;
    private bool _initialized;

    [Inject] public HttpClient HttpClient { get; set; } = null!;
    [Inject] public ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] public Registry Registry { get; set; } = null!;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = null!;

    [Parameter] public string Icon { get; set; } = string.Empty;

    [Parameter] public string Color { get; set; } = string.Empty;
    

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _initialized = true;
        
        if (!firstRender) return;

        if (string.IsNullOrEmpty(Icon))
        {
            // Fallback to error icon if no icon is provided
            Icon = ErrorIcon;
            return;
        }

        // Only fetch the icon if it has changed
        if (await Registry.IsCached(Icon, Color))
        {
            var metadata = await Registry.GetIcon(Icon, Color);
            if (metadata is null) return;

            var svg = TryParseToXml(metadata.Content);
            if (svg is null) return;

            UpdateSvg(svg);
        }
        else
        { 
            string iconUrl = $"{API}{Icon.Replace(':', '/')}.svg";
            if (!string.IsNullOrEmpty(Color))
            {
                iconUrl += $"?color={UrlEncoder.Default.Encode(Color)}";
            }
            _svg = await FetchIconAsync(iconUrl);

            if (string.IsNullOrEmpty(_svg))
            {
                Console.WriteLine($"Failed to fetch icon {(!string.IsNullOrEmpty(Icon) ? Icon : "\"null\"")}");
                return;
            }

            var svg = TryParseToXml(_svg);
            if (svg is null) return;

            UpdateSvg(svg);

            await Registry.AddIcon(new IconMetaData
            {
                Name = Icon,
                Content = _svg,
                Color = Color,
                TimeFetched = DateTime.Now
            });
        }

        if (string.IsNullOrEmpty(_svg))
            Console.WriteLine($"Failed to fetch icon {this}");

        StateHasChanged();
        Console.WriteLine("RENDER ICONIFY");
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_initialized) return;
        await OnAfterRenderAsync(true);
    }

    private async Task<string> FetchIconAsync(string url)
    {
        if (string.IsNullOrEmpty(Icon)) return string.Empty;

        var response = await HttpClient.GetByteArrayAsync(url);
        var iconContents = Encoding.UTF8.GetString(response);

        if (iconContents is not "404" && response is not ({ Length: 0 } or null))
            return iconContents;

        iconContents = string.Empty;
        return iconContents;
    }

    private static XmlDocument? TryParseToXml(string content)
    {
        try
        {
            var document = new XmlDocument();
            document.LoadXml(content);

            return document;
        }
        catch (XmlException ex)
        {
            Console.WriteLine("Failed to parse xml from svg file.");
        }

        return null;
    }

    private void UpdateSvg(XmlDocument document)
    {
        var rootElement = document.DocumentElement;

        switch (rootElement)
        {
            case null:
                Console.WriteLine("No root element.");
                return;
            case not { Name: "svg" } or null:
                Console.WriteLine("Failed to find svg element.");
                return;
        }

        if (rootElement is null)
        {
            Console.WriteLine("Failed to find svg element.");
            return;
        }

        rootElement.SetAttribute("class", $"{Attributes.Get("i-class")} icon");
        rootElement.SetAttribute("style", Attributes.Get("i-style"));
        rootElement.RemoveAttribute("width");
        rootElement.RemoveAttribute("height");

        foreach (XmlElement child in rootElement.ChildNodes)
        {
            if (child.Name.ToLower() is not "path") continue;
        }

        _svg = rootElement.OuterXml;
    }
}