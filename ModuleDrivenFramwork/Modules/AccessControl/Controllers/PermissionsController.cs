using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleDrivenFramwork.Modules.AccessControl.Application.DTOs;
using ModuleDrivenFramwork.Modules.AccessControl.Services;

namespace ModuleDrivenFramwork.Modules.AccessControl.Controllers;

[ApiController]
[Route("api/permissions")]
[AllowAnonymous]
public class PermissionsController : ControllerBase
{
    private readonly IAccessControlService _accessControl;

    public PermissionsController(IAccessControlService accessControl)
    {
        _accessControl = accessControl;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetAll()
    {
        return Ok(await _accessControl.GetPermissionsAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PermissionDto>> GetById(Guid id)
    {
        var permission = await _accessControl.GetPermissionAsync(id);
        return permission == null ? NotFound() : Ok(permission);
    }

    [HttpPost]
    public async Task<ActionResult<PermissionDto>> Create([FromBody] CreatePermissionRequestDto request)
    {
        try
        {
            var permission = await _accessControl.CreatePermissionAsync(request);
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
            return Ok(await _accessControl.UpdatePermissionAsync(id, request));
        }
        catch (InvalidOperationException exception) when (exception.Message == "Permission not found.")
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
            await _accessControl.DeletePermissionAsync(id);
            return NoContent();
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
}
