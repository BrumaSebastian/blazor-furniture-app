using BlazorFurniture.Shared.Services.API;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorFurniture.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    extension( IServiceCollection services )
    {
        public IHttpClientBuilder AddApiClients( string? baseAddress = null )
        {
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() },
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            };

            var builder = services.AddRefitClient<IUserApi>(refitSettings);

            if (baseAddress is not null)
            {
                builder.ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));
            }

            return builder;
        }
    }
}
