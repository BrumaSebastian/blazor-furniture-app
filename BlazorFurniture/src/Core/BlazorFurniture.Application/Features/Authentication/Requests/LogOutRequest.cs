using FastEndpoints;

namespace BlazorFurniture.Application.Features.Authentication.Requests;

public class LogOutRequest
{
    [QueryParam]
    public string? RedirectUrl { get; set; }
}
