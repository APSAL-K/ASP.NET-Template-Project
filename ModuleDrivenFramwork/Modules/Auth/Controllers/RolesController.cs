using Microsoft.AspNetCore.Mvc;
using ModuleDrivenFramwork.Modules.Auth.Application.DTOs.AuthManagement;
using ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

namespace ModuleDrivenFramwork.Modules.Auth.Controllers;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IAuthManagementService _authManagementService;

    public RolesController(IAuthManagementService authManagementService)
    {
        _authManagementService = authManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll()
    {
        return Ok(await _authManagementService.GetRolesAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoleDto>> GetById(Guid id)
    {
        var role = await _authManagementService.GetRoleAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequestDto request)
    {
        try
        {
            var role = await _authManagementService.CreateRoleAsync(request);
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
            return Ok(await _authManagementService.UpdateRoleAsync(id, request));
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _authManagementService.DeleteRoleAsync(id);
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