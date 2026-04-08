using Microsoft.AspNetCore.Mvc;
using ModuleDrivenFramwork.Modules.Auth.Application.DTOs.AuthManagement;
using ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

namespace ModuleDrivenFramwork.Modules.Auth.Controllers;

[ApiController]
[Route("api/permissions")]
public class PermissionsController : ControllerBase
{
    private readonly IAuthManagementService _authManagementService;

    public PermissionsController(IAuthManagementService authManagementService)
    {
        _authManagementService = authManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetAll()
    {
        return Ok(await _authManagementService.GetPermissionsAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PermissionDto>> GetById(Guid id)
    {
        var permission = await _authManagementService.GetPermissionAsync(id);
        return permission == null ? NotFound() : Ok(permission);
    }

    [HttpPost]
    public async Task<ActionResult<PermissionDto>> Create([FromBody] CreatePermissionRequestDto request)
    {
        try
        {
            var permission = await _authManagementService.CreatePermissionAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PermissionDto>> Update(Guid id, [FromBody] UpdatePermissionRequestDto request)
    {
        try
        {
            return Ok(await _authManagementService.UpdatePermissionAsync(id, request));
        }
        catch (InvalidOperationException exception) when (exception.Message == "Permission not found.")
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
            await _authManagementService.DeletePermissionAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException exception) when (exception.Message == "Permission not found.")
        {
            return NotFound(new { error = exception.Message });
        }
    }
}