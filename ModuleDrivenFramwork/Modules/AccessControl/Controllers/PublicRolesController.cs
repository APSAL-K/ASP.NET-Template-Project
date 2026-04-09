using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleDrivenFramwork.Modules.AccessControl.Services;

namespace ModuleDrivenFramwork.Modules.AccessControl.Controllers;

[ApiController]
[Route("api/public/roles")]
[AllowAnonymous]
public class PublicRolesController : ControllerBase
{
    private readonly IAccessControlService _accessControl;

    public PublicRolesController(IAccessControlService accessControl)
    {
        _accessControl = accessControl;
    }

    [HttpGet]
    public async Task<IActionResult> GetPublicRoles()
    {
        // For now, return all roles. In a real system, you might filter these.
        var result = await _accessControl.GetRolesAsync(1, 100);
        return Ok(result.Items.Select(r => new { r.Id, r.Name, r.Description }));
    }
}
