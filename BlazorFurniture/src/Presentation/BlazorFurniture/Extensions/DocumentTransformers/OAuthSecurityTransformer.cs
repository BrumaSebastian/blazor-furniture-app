using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace BlazorFurniture.Extensions.DocumentTransformers;

public class OAuthSecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync( OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken )
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                ClientCredentials = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("http://localhost:8080/realms/main/protocol/openid-connect/token")
                }
            }
        };
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add("OAuth2", securityScheme);
        return Task.CompletedTask;
    }
}
