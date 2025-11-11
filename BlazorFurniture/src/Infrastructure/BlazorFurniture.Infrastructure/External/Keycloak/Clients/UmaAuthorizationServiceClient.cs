using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients;

internal class UmaAuthorizationServiceClient( Endpoints endpoints, HttpClient httpClient, KeycloakConfiguration configuration, IMemoryCache cache )
    : KeycloakBaseHttpClient(endpoints, httpClient, configuration, cache), IUmaAuthorizationService
{
    public async Task<HttpResult<List<UmaPermissionsResponse>, ErrorRepresentation>> CheckPermissions( string accessToken, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Post)
           .WithPath(Endpoints.AccessToken())
           .WithFormParams(new Dictionary<string, string>
            {
                { OpenIdConnectParameterNames.GrantType, UmaAuthorizationConstants.UMA_TICKET_GRANT_TYPE },
                { OpenIdConnectParameterNames.ResponseMode, UmaAuthorizationConstants.UMA_RESPONSE_MODE_PERMISSIONS },
                { UmaAuthorizationConstants.AUDIENCE_PARAM, Configuration.ServiceClient.ClientId }
            })
           .Build();

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

        var response = await HttpClient.SendAsync(requestMessage, ct);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<List<UmaPermissionsResponse>>(cancellationToken: ct);

            return HttpResult<List<UmaPermissionsResponse>, ErrorRepresentation>.Succeeded(content!);
        }

        return HttpResult<List<UmaPermissionsResponse>, ErrorRepresentation>.Failed(new ErrorRepresentation()
        {
            Error = "UMA_PERMISSION_CHECK_FAILED",
            Description = $"UMA permission check failed with status code: {response.StatusCode}"
        }, response.StatusCode);
    }

    public async Task<HttpResult<EmptyResult, ErrorRepresentation>> Evaluate( string accessToken, string permission, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Post)
           .WithPath(Endpoints.AccessToken())
           .WithFormParams(new Dictionary<string, string>
            {
                { OpenIdConnectParameterNames.GrantType, UmaAuthorizationConstants.UMA_TICKET_GRANT_TYPE },
                { OpenIdConnectParameterNames.ResponseMode, UmaAuthorizationConstants.UMA_RESPONSE_MODE_DECISION },
                { UmaAuthorizationConstants.UMA_PERMISSION_PARAM, permission },
                { UmaAuthorizationConstants.AUDIENCE_PARAM, Configuration.ServiceClient.ClientId }
            })
           .Build();

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

        var response = await HttpClient.SendAsync(requestMessage, ct);

        return response.IsSuccessStatusCode
            ? HttpResult<EmptyResult, ErrorRepresentation>.Succeeded(new EmptyResult())
            : HttpResult<EmptyResult, ErrorRepresentation>.Failed(new ErrorRepresentation()
            {
                Error = "UMA_EVALUATION_FAILED",
                Description = $"UMA authorization evaluation failed with status code: {response.StatusCode}"
            }, response.StatusCode);
    }
}
