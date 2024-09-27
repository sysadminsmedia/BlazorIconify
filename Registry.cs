using Blazored.LocalStorage;

namespace SysAdminsMedia.BlazorIconify;

public sealed class Registry(ILocalStorageService LocalStorage)
{
    private const string CachedIconsKey = "cached-icons";

    private List<IconMetaData> _icons = [];

    public async Task AddIcon(IconMetaData metadata)
    {
        if(string.IsNullOrEmpty(metadata.Name)) return;
        if (IsRegistered(metadata.Name)) return;

        _icons.Add(metadata);
        await LocalStorage.SetItemAsync(CachedIconsKey, _icons);
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
        await LocalStorage.RemoveItemAsync(CachedIconsKey);
    }

    private async Task<List<IconMetaData>> GetCachedIcons()
    {
        if (_icons.Count > 0) return _icons;
        return _icons = await LocalStorage.GetItemAsync<List<IconMetaData>>(CachedIconsKey) ?? [];
    }

    private bool IsRegistered(string icon) =>
        _icons.Exists(x => x.Name == icon);
}