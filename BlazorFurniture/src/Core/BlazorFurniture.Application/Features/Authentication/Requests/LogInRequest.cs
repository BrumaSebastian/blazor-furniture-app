using FastEndpoints;

namespace BlazorFurniture.Application.Features.Authentication.Requests;

public class LogInRequest
{
    [QueryParam]
    public string? RedirectUrl { get; set; }
}
