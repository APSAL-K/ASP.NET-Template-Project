using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleDrivenFramwork.Modules.Auth.Application.DTOs.AuthManagement;
using ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

namespace ModuleDrivenFramwork.Modules.Auth.Controllers;

[ApiController]
[Route("api/users")]
[AllowAnonymous]
public class UsersController : ControllerBase
{
    private readonly IAuthManagementService _authManagementService;

    public UsersController(IAuthManagementService authManagementService)
    {
        _authManagementService = authManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll()
    {
        return Ok(await _authManagementService.GetUsersAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _authManagementService.GetUserAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequestDto request)
    {
        try
        {
            var user = await _authManagementService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserRequestDto request)
    {
        try
        {
            return Ok(await _authManagementService.UpdateUserAsync(id, request));
        }
        catch (InvalidOperationException exception) when (exception.Message == "User not found.")
        {
            return NotFound(new { error = exception.Message });
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("modified or deleted"))
        {
            return Conflict(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _authManagementService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException exception) when (exception.Message == "User not found.")
        {
            return NotFound(new { error = exception.Message });
        }
    }
}