using System.Threading.Tasks;
using ModuleDrivenFramwork.Modules.Auth.Application.DTOs.Auth;

namespace ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, string ipAddress = "");
    Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO request, string ipAddress = "");
    Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken, string ipAddress = "");
}
