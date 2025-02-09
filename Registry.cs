using Blazored.LocalStorage;
using Microsoft.Extensions.Options;
using SysAdminsMedia.BlazorIconify.Extensions;

namespace SysAdminsMedia.BlazorIconify;

public sealed class Registry(IOptions<IconifyOptions> options, ILocalStorageService localStorage)
{
    private const string CachedIconsKey = "cached-icons";

    private List<IconMetaData> _icons = [];

    public string GetApiUrl()
    {
        return options.Value.ApiUrl ?? "https://api.iconify.design/";
    }

    public string GetDefaultColor()
    {
        return options.Value.DefaultColor ?? string.Empty;
    }
    
    public string GetErrorIcon()
    {
        return options.Value.ErrorIcon ?? "ic:baseline-do-not-disturb";
    }

    public async Task AddIcon(IconMetaData metadata)
    {
        if(string.IsNullOrEmpty(metadata.Name)) return;
        if (IsRegistered(metadata.Name)) return;

        _icons.Add(metadata);
        await localStorage.SetItemAsync(CachedIconsKey, _icons);
    }

    public async Task<IconMetaData?> GetIcon(string icon, string? color = "")
    {
        if (string.IsNullOrEmpty(icon)) return null;
        
        var icons = await GetCachedIcons();
        return icons.FirstOrDefault(x => x.Name == icon && x.Color == color);
    }
    
    public async Task<bool> IsCached(string icon, string? color = "")
    {
        if (string.IsNullOrEmpty(icon)) return false;
        
        var icons = await GetCachedIcons();
        return icons.Exists(x => x.Name == icon && x.Color == color);
    }

    public async Task Clear()
    {
        _icons.Clear();
        await localStorage.RemoveItemAsync(CachedIconsKey);
    }

    private async Task<List<IconMetaData>> GetCachedIcons()
    {
        if (_icons.Count > 0) return _icons;
        return _icons = await localStorage.GetItemAsync<List<IconMetaData>>(CachedIconsKey) ?? [];
    }

    private bool IsRegistered(string icon) =>
        _icons.Exists(x => x.Name == icon);
}