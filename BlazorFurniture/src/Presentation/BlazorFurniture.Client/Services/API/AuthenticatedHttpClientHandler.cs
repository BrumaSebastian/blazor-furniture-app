using Microsoft.AspNetCore.Components;
using System.Net;

namespace BlazorFurniture.Client.Services.API;

public class AuthenticatedHttpClientHandler( NavigationManager navigationManager ) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken )
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Redirect to login
            navigationManager.NavigateTo("/api/authentication/login", forceLoad: true);
        }

        return response;
    }
}
