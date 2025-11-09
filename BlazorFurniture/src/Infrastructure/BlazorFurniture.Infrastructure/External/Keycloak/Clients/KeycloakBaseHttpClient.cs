using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net;
using System.Net.Http.Headers;
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

    protected async Task<HttpResult<TValue, ErrorRepresentation>> SendRequest<TValue>( HttpRequestMessage requestMessage, CancellationToken ct )
        where TValue : class, new()
    {
        var tokenResult = await GetServiceAccessToken(ct);

        if (!tokenResult.TryGetValue(out var token))
        {
            return HttpResult<TValue, ErrorRepresentation>.Failed(tokenResult.Error, tokenResult.StatusCode);
        }

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.AccessToken);

        using var response = await HttpClient.SendAsync(requestMessage, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorRepresentation>(ct);

            return HttpResult<TValue, ErrorRepresentation>.Failed(error!, response.StatusCode);
        }

        if (response.StatusCode is HttpStatusCode.NoContent)
        {
            return HttpResult<TValue, ErrorRepresentation>.NoContent();
        }

        if (response.StatusCode is HttpStatusCode.Created)
        {
            return HttpResult<TValue, ErrorRepresentation>.Created(response.Headers.Location);
        }

        var result = await response.Content.ReadFromJsonAsync<TValue>(ct);

        return result is null
            ? throw new Exception("Failed to deserialize response.")
            : HttpResult<TValue, ErrorRepresentation>.Succeeded(result);
    }

    protected async Task<HttpResult<KeycloakAccessToken, ErrorRepresentation>> GetServiceAccessToken( CancellationToken ct )
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
                var error = await response.Content.ReadFromJsonAsync<ErrorRepresentation>(ct);

                return HttpResult<KeycloakAccessToken, ErrorRepresentation>.Failed(error!, response.StatusCode);
            }

            var token = await response.Content.ReadFromJsonAsync<KeycloakAccessToken>(ct)
                ?? throw new Exception("Failed read service token.");

            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.ExpiresIn - TOKEN_EXPIRATION_BUFFER);

            return HttpResult<KeycloakAccessToken, ErrorRepresentation>.Succeeded(token);
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
