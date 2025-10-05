using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BlazorFurniture.Extensions.DocumentTransformers;

public class OAuthSecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync( OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken )
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes.Add(nameof(SecuritySchemeType.OAuth2), new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            In = ParameterLocation.Header
        });

        return Task.CompletedTask;
    }
}
