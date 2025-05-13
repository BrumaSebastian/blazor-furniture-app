using BlazorFurniture.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFurniture.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly KeycloakService _keycloakService;

    public UserController(KeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        await _keycloakService.CreateUserAsync(request.Username, request.Email, request.FirstName, request.LastName);

        return Ok(new { message = "User created successfully." });
    }
}

public record CreateUserRequest(string Username, string Email, string FirstName, string LastName);