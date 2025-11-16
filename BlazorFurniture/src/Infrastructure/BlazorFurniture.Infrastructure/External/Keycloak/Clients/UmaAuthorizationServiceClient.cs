using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Json;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients;

internal class UmaAuthorizationServiceClient( Endpoints endpoints, HttpClient httpClient, KeycloakConfiguration configuration, IMemoryCache cache, ILogger<UmaAuthorizationServiceClient> logger )
    : KeycloakBaseHttpClient(endpoints, httpClient, configuration, cache), IUmaAuthorizationService
{
    public async Task<HttpResult<List<UmaPermissionsResponse>, ErrorRepresentation>> CheckPermissions( string userAccessToken, CancellationToken ct )
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
           .WithBearerAuthorization(userAccessToken)
           .Build();

        var response = await HttpClient.SendAsync(requestMessage, ct);

        if (!response.IsSuccessStatusCode)
        {
            return HttpResult<List<UmaPermissionsResponse>, ErrorRepresentation>.Failed(new ErrorRepresentation()
            {
                Error = "UMA_PERMISSION_CHECK_FAILED",
                Description = $"UMA permission check failed with status code: {response.StatusCode}"
            }, response.StatusCode);
        }

        var content = await response.Content.ReadFromJsonAsync<List<UmaPermissionsResponse>>(cancellationToken: ct);

        return HttpResult<List<UmaPermissionsResponse>, ErrorRepresentation>.Succeeded(content ?? throw new Exception("Couldn't serialize the uma permissions response"));
    }

    public async Task<HttpResult<UmaAuthorizationResponse, ErrorRepresentation>> Evaluate( string userAccessToken, string resource, List<string> scopes, CancellationToken ct )
    {
        var permission = CreatePermission(resource, scopes);
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
           .WithBearerAuthorization(userAccessToken)
           .Build();

        var response = await HttpClient.SendAsync(requestMessage, ct);

        if (!response.IsSuccessStatusCode)
        {
            return HttpResult<UmaAuthorizationResponse, ErrorRepresentation>.Failed(new ErrorRepresentation()
            {
                Error = "UMA_EVALUATION_FAILED",
                Description = $"UMA authorization evaluation failed with status code: {response.StatusCode}"
            }, response.StatusCode);
        }

        var content = await response.Content.ReadFromJsonAsync<UmaAuthorizationResponse>(ct);

        return HttpResult<UmaAuthorizationResponse, ErrorRepresentation>.Succeeded(content ?? throw new Exception("Couldn't serialize the uma authorization response"));
    }

    public async Task<HttpResult<UmaAuthorizationResponse, ErrorRepresentation>> Evaluate( string userAccessToken, string resource, List<string> scopes, IReadOnlyDictionary<string, List<string>> claims, CancellationToken ct )
    {
        var authClientAccessTokenResult = await GetServiceAccessToken(ct);

        if (authClientAccessTokenResult.IsFailure)
        {
            return HttpResult<UmaAuthorizationResponse, ErrorRepresentation>.Failed(authClientAccessTokenResult.Error, authClientAccessTokenResult.StatusCode);
        }

        var ticketRequest = new UmaPermissionTicketRequest
        {
            ResourceId = resource,
            ResourceScopes = scopes,
            Claims = (IDictionary<string, List<string>>)claims
        };

        var ticketResult = await GetPermissionTicket(authClientAccessTokenResult.Value.AccessToken!, ticketRequest, ct);

        if (ticketResult.IsFailure)
        {
            return HttpResult<UmaAuthorizationResponse, ErrorRepresentation>.Failed(ticketResult.Error, ticketResult.StatusCode);
        }

        var authorizationResult = await EvaluateWithTicket(userAccessToken, ticketResult.Value.Ticket, ct);

        if (authorizationResult.IsFailure)
        {
            return HttpResult<UmaAuthorizationResponse, ErrorRepresentation>.Failed(authorizationResult.Error, authorizationResult.StatusCode);
        }

        return authorizationResult;
    }

    private static string CreatePermission( string resource, IEnumerable<string> scopes )
    {
        return !scopes.Any()
            ? resource
            : $"{resource}#{string.Join(", ", scopes)}";
    }

    private async Task<HttpResult<UmaPermissionTicketResponse, ErrorRepresentation>> GetPermissionTicket( string authClientAccessToken, UmaPermissionTicketRequest ticketRequest, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Post)
           .WithPath(Endpoints.AuthzPermission())
           .WithContent(new List<UmaPermissionTicketRequest>(1)
           {
               ticketRequest
           })
           .WithBearerAuthorization(authClientAccessToken)
           .Build();

        var response = await HttpClient.SendAsync(requestMessage, ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogInformation("Creation of ticket failed, {Content}", await response.Content.ReadAsStringAsync(ct));

            return HttpResult<UmaPermissionTicketResponse, ErrorRepresentation>.Failed(new ErrorRepresentation()
            {
                Error = "UMA_TICKET_REQUEST_FAILED",
                Description = $"UMA permission ticket request failed with status code: {response.StatusCode}"
            }, response.StatusCode);
        }

        var content = await response.Content.ReadFromJsonAsync<UmaPermissionTicketResponse>(cancellationToken: ct);

        return HttpResult<UmaPermissionTicketResponse, ErrorRepresentation>.Succeeded(content ?? throw new Exception("Couldn't serialize the uma ticket response"));
    }

    private async Task<HttpResult<UmaAuthorizationResponse, ErrorRepresentation>> EvaluateWithTicket( string accessToken, string ticket, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
          .Create(HttpClient, HttpMethod.Post)
          .WithPath(Endpoints.AccessToken())
          .WithFormParams(new Dictionary<string, string>
           {
                { OpenIdConnectParameterNames.GrantType, UmaAuthorizationConstants.UMA_TICKET_GRANT_TYPE },
                { OpenIdConnectParameterNames.ResponseMode, UmaAuthorizationConstants.UMA_RESPONSE_MODE_DECISION },
                { UmaAuthorizationConstants.UMA_TICKET_PARAM, ticket }
           })
          .WithBearerAuthorization(accessToken)
          .Build();

        var response = await HttpClient.SendAsync(requestMessage, ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogInformation("Evaluation of ticket failed, {Content}", await response.Content.ReadAsStringAsync(ct));

            return HttpResult<UmaAuthorizationResponse, ErrorRepresentation>.Failed(new ErrorRepresentation()
            {
                Error = "UMA_EVALUATION_FAILED",
                Description = $"UMA authorization evaluation failed with status code: {response.StatusCode}"
            }, response.StatusCode);

        }

        var content = await response.Content.ReadFromJsonAsync<UmaAuthorizationResponse>(cancellationToken: ct);

        return HttpResult<UmaAuthorizationResponse, ErrorRepresentation>.Succeeded(content ?? throw new Exception("Couldn't serialize the uma authorization response"));
    }
}
