using System.Text.Json.Serialization;

namespace BlazorFurniture.Modules.Keycloak.Models;

public class RealmRepresentation
{
    //public string Id { get; set; }
    [JsonPropertyName("realm")]
    public string Realm { get; set; }
    //public string DisplayName { get; set; }
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}
