using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Core.Shared.Models.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Json;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients;

internal abstract class KeycloakBaseHttpClient(
    Endpoints endpoints,
    HttpClient httpClient,
    KeycloakConfiguration configuration,
    IMemoryCache Cache )
{
    private const string SERVICE_TOKEN_CACHE_KEY = "ServiceTokenCache";
    private const int TOKEN_EXPIRATION_BUFFER = 60;

    protected Endpoints Endpoints { get; } = endpoints;
    protected HttpClient HttpClient { get; } = httpClient;

    protected async Task<Result<T>> SendRequest<T>( HttpRequestMessage requestMessage, CancellationToken ct ) where T : class
    {
        var tokenResult = await GetServiceAccessToken(ct);

        if (!tokenResult.TryGetValue(out var token))
        {
            return Result<T>.Failed(tokenResult.Error!);
        }

        requestMessage.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.AccessToken);

        using var response = await HttpClient.SendAsync(requestMessage, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<KeycloakError>(ct);

            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => new NotFoundError("", ""),
                System.Net.HttpStatusCode.BadRequest => new ValidationError(new Dictionary<string, string[]>()),
                _ => new GenericError(error?.Title ?? error?.Description ?? "unexpected error")
            };
        }

        // TODO: deserialize to the value T
        var result = await response.Content.ReadFromJsonAsync<T>(ct);

        if (result == null)
        {
            throw new Exception("Failed to deserialize response.");
        }

        return Result<T>.Succeeded(result);
    }

    protected async Task<Result<KeycloakAccessToken>> GetServiceAccessToken( CancellationToken ct )
    {
        return await Cache.GetOrCreateAsync(SERVICE_TOKEN_CACHE_KEY, async entry =>
        {
            var requestMessage = HttpRequestMessageBuilder.Create(HttpClient, HttpMethod.Post)
                .WithPath(Endpoints.AccessToken())
                .WithFormParams(new Dictionary<string, string>
                {
                    { OpenIdConnectParameterNames.GrantType, OpenIdConnectGrantTypes.ClientCredentials },
                    { OpenIdConnectParameterNames.ClientId, configuration.ServiceClient.ClientId },
                    { OpenIdConnectParameterNames.ClientSecret, configuration.ServiceClient.ClientSecret }
                })
                .Build();

            using var response = await HttpClient.SendAsync(requestMessage, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<KeycloakError>(ct);

                return response.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => new ValidationError(new Dictionary<string, string[]>()),
                    _ => new GenericError(error?.Title ?? error?.Description ?? "unexpected error")
                };
            }

            var token = await response.Content.ReadFromJsonAsync<KeycloakAccessToken>(ct)
                ?? throw new Exception("Failed read service token.");

            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.ExpiresIn - TOKEN_EXPIRATION_BUFFER);

            return Result<KeycloakAccessToken>.Succeeded(token);
        }) ?? throw new Exception("Failed to retrieve service token");
    }

    //public async Task<Result<KeycloakToken>> GetUserTokenAsync( string username, string password, CancellationToken cancellationToken )
    //{
    //    var requestBody = new FormUrlEncodedContent(
    //            new Dictionary<string, string>
    //            {
    //                { OpenIdConnectParameterNames.GrantType, OpenIdConnectGrantTypes.Password },
    //                { OpenIdConnectParameterNames.ClientId, configuration.ServiceClient.ClientId },
    //                { OpenIdConnectParameterNames.ClientSecret, configuration.ServiceClient.ClientSecret },
    //                { OpenIdConnectParameterNames.Username,  username },
    //                { OpenIdConnectParameterNames.Password,  password }
    //            });

    //    using var response = await httpClient.PostAsync(endpoints.GetAccessToken(), requestBody, cancellationToken);

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        var error = await response.Content.ReadFromJsonAsync<KeycloakError>(cancellationToken);
    //        return Result<KeycloakToken>.Failure(error, response.StatusCode);
    //    }

    //    var token = await response.Content.ReadFromJsonAsync<KeycloakToken>(cancellationToken)
    //        ?? throw new Exception("Failed to read service token.");

    //    return Result<KeycloakToken>.Success(token, response.StatusCode);
    //}
}
