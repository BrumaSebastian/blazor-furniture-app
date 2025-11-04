namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record GroupRepresentation
{
    public const string DESCRIPTION_ATTRIBUTE = "description";
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Dictionary<string, IEnumerable<string>> Attributes { get; set; } = [];
}
