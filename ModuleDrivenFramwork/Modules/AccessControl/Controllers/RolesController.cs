using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuleDrivenFramwork.Modules.AccessControl.Application.DTOs;
using ModuleDrivenFramwork.Modules.AccessControl.Services;

namespace ModuleDrivenFramwork.Modules.AccessControl.Controllers;

[ApiController]
[Route("api/roles")]
[AllowAnonymous]
public class RolesController : ControllerBase
{
    private readonly IAccessControlService _accessControl;

    public RolesController(IAccessControlService accessControl)
    {
        _accessControl = accessControl;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll()
    {
        return Ok(await _accessControl.GetRolesAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoleDto>> GetById(Guid id)
    {
        var role = await _accessControl.GetRoleAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequestDto request)
    {
        try
        {
            var role = await _accessControl.CreateRoleAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoleDto>> Update(Guid id, [FromBody] UpdateRoleRequestDto request)
    {
        try
        {
            return Ok(await _accessControl.UpdateRoleAsync(id, request));
        }
        catch (InvalidOperationException exception) when (exception.Message == "Role not found.")
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
            await _accessControl.DeleteRoleAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException exception) when (exception.Message == "Role not found.")
        {
            return NotFound(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}
