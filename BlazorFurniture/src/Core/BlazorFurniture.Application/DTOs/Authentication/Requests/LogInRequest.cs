using FastEndpoints;

namespace BlazorFurniture.Application.DTOs.Authentication.Requests;

public class LogInRequest
{
    [QueryParam]
    public string? RedirectUrl { get; set; }
}
