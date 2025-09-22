namespace BlazorFurniture.Core.Shared.Configurations;

public class OpenIdConectOptions
{
    public static readonly string NAME = "OpenIdConnect";
    public required string Authority { get; set; }
    public PublicClient? DevPublicClient { get; set; }
    public required PublicClient PublicClient { get; set; }
    public required ConfidentialClient ConfidentialClient { get; set; }
}


public class PublicClient
{
    public required string ClientId { get; set; }
    public List<string> Scopes { get; set; } = [];
}

public class ConfidentialClient
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}
