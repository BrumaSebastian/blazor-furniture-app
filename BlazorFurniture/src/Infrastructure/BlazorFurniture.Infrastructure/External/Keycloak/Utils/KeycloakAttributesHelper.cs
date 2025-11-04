namespace BlazorFurniture.Infrastructure.External.Keycloak.Utils;

internal static class KeycloakAttributesHelper
{
    public static string GetAttributeValue(
        Dictionary<string, IEnumerable<string>> source,
        string attributeName,
        string defaultValue = "" )
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.TryGetValue(attributeName, out var values) && values.Any())
        {
            return values.First();
        }

        return defaultValue;
    }

    public static int GetAttributeValueAsInt( 
        Dictionary<string, IEnumerable<string>> source, 
        string attributeName, 
        int defaultValue = 0 )
    {
        string value = GetAttributeValue(source, attributeName);

        return int.TryParse(value, out int result) ? result : defaultValue;
    }

    public static bool GetAttributeValueAsBool( 
        Dictionary<string, IEnumerable<string>> source, 
        string attributeName, 
        bool defaultValue = false )
    {
        string value = GetAttributeValue(source, attributeName);

        return bool.TryParse(value, out bool result) ? result : defaultValue;
    }

    public static DateTime GetAttributeValueAsDateTime( 
        Dictionary<string, IEnumerable<string>> source, 
        string attributeName, 
        DateTime defaultValue = default )
    {
        string value = GetAttributeValue(source, attributeName);

        return DateTime.TryParse(value, out var result) ? result : defaultValue;
    }
}
