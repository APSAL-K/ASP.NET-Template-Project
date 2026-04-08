using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModuleDrivenFramwork.Modules.Auth.Application.DTOs.Auth;
using ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

namespace ModuleDrivenFramwork.Modules.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO req)
    {
        try
        {
            var result = await _auth.LoginAsync(req);
            return Ok(result);
        }
        catch
        {
            return Unauthorized();
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO req)
    {
        try
        {
            var result = await _auth.RegisterAsync(req);
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO req)
    {
        try
        {
            var result = await _auth.RefreshTokenAsync(req.RefreshToken);
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
