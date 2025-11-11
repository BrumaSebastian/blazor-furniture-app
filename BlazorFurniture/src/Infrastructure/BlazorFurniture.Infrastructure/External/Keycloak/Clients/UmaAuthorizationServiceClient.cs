using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients;

internal class UmaAuthorizationServiceClient( Endpoints endpoints, HttpClient httpClient, KeycloakConfiguration configuration, IMemoryCache cache )
    : KeycloakBaseHttpClient(endpoints, httpClient, configuration, cache), IUmaAuthorizationService
{
    private const string UMA_TICKET_GRANT_TYPE = "urn:ietf:params:oauth:grant-type:uma-ticket";
    private const string UMA_RESPONSE_MODE_DECISION = "decision";
    private const string UMA_PERMISSION_PARAM = "permission";
    private const string AUDIENCE_PARAM = "audience";

    public async Task<HttpResult<EmptyResult, ErrorRepresentation>> Evaluate( string accessToken, string permission, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Post)
           .WithPath(Endpoints.AccessToken())
           .WithFormParams(new Dictionary<string, string>
            {
                { OpenIdConnectParameterNames.GrantType, UMA_TICKET_GRANT_TYPE },
                { OpenIdConnectParameterNames.ResponseMode, UMA_RESPONSE_MODE_DECISION },
                { UMA_PERMISSION_PARAM, permission },
                { AUDIENCE_PARAM, Configuration.ServiceClient.ClientId }
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
