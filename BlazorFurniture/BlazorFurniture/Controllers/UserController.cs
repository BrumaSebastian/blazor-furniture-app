using BlazorFurniture.Common.Dispatchers;
using BlazorFurniture.Modules.Keycloak.Models;
using BlazorFurniture.Modules.Keycloak.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFurniture.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IQueryDispatcher queryDispatcher) : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher = queryDispatcher;

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        //await _keycloakService.CreateUserAsync(request.Username, request.Email, request.FirstName, request.LastName);

        return Ok(new { message = "User created successfully." });
    }

    [HttpGet]
    [Authorize(Roles = "RealmAdmin")]
    public async Task<IActionResult> Get()
    {
        var users = await _queryDispatcher.DispatchQueryAsync<GetUsersQuery, List<User>>(new GetUsersQuery());

        return Ok();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Get(Guid id)
    {
        return Ok();
    }

    [HttpPut]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Update()
    {

        return Ok();
    }

    [HttpPut("{realm}/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string realm, Guid id)
    {

        return Ok();
    }

    [HttpDelete("{realm}/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string realm, Guid id)
    {

        return Ok();
    }

    //[HttpGet]
    //public async Task<IActionResult> ResetPasswordSms()
    //{
    //    var users = await _keycloakService.GetUsersAsync();

    //    return Ok();
    //}
}

public record CreateUserRequest(string Username, string Email, string FirstName, string LastName);