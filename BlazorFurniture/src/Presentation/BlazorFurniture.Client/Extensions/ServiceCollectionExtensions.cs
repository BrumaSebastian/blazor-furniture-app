namespace BlazorFurniture.Client.Extensions;

public static class ServiceCollectionExtensions
{
    //extension( IServiceCollection services )
    //{
    //    public IServiceCollection AddApiClients( string baseAddress )
    //    {
    //        var refitSettings = new RefitSettings
    //        {
    //            ContentSerializer = new SystemTextJsonContentSerializer(
    //            new JsonSerializerOptions
    //            {
    //                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    //                Converters = { new JsonStringEnumConverter() },
    //                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    //            })
    //        };

    //        var uri = new Uri(baseAddress);

    //        services.AddRefitClient<IUserApi>(refitSettings)
    //            .ConfigureHttpClient(c => c.BaseAddress = uri);

    //        return services;
    //    }
    //}
}
