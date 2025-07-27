using BlazorFurniture.Common.Dispatchers;
using BlazorFurniture.Modules.Keycloak.Commands;
using BlazorFurniture.Modules.Keycloak.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFurniture.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RealmController(ICommandDispatcher commandDispatcher) : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;

    [HttpPost]
    [Authorize(Roles = "RealmAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateRealmRequest request)
    {
        var realm = new RealmRepresentation { Realm = request.Name, Enabled = true };
        await _commandDispatcher.DispatchAsync(new CreateRealmCommand(realm));

        return Ok(new { message = "User created successfully." });
    }
}
