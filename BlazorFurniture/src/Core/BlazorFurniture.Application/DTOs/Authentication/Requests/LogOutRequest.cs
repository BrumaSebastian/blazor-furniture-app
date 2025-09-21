using FastEndpoints;

namespace BlazorFurniture.Application.DTOs.Authentication.Requests;

public class LogOutRequest
{
    [QueryParam]
    public string? RedirectUrl { get; set; }
}
