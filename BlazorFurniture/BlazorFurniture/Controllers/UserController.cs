using BlazorFurniture.Common.Dispatchers;
using BlazorFurniture.Modules.Keycloak.Commands.Update;
using BlazorFurniture.Modules.Keycloak.Models;
using BlazorFurniture.Modules.Keycloak.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFurniture.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher) : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher = queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;

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

    //[HttpPut]
    //[Authorize(Roles = "Admin,User")]
    //public async Task<IActionResult> Update()
    //{

    //    return Ok();
    //}

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

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> ResetPassword(UpdatePasswordRequest req)
    {
        await _commandDispatcher.DispatchAsync(new UpdatePasswordCommand(req, HttpContext.Items["JwtEmail"].ToString(), HttpContext.Items["JwtUserId"].ToString()));

        return Ok();
    }
}

public record CreateUserRequest(string Username, string Email, string FirstName, string LastName);