namespace SysAdminsMedia.BlazorIconify.Extensions;

public static class DictionaryExtension
{
    public static string Get(this Dictionary<string, object> data, string className)
    {
        if (data is null)
            return string.Empty;

        return data.TryGetValue(className, out var value) ? value.ToString()! : string.Empty;
    }
}