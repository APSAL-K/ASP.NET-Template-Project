using System.Threading.Tasks;
using MyApi.Modules.Auth.Application.DTOs.Auth;

namespace MyApi.Modules.Auth.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, string ipAddress = "");
    Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO request, string ipAddress = "");
    Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken, string ipAddress = "");
}
