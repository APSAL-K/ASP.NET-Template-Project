using System;
using System.Threading.Tasks;
using MyApi.Modules.Auth.Application.DTOs.Auth;
using MyApi.Modules.Auth.Application.Interfaces;
using MyApi.Modules.Auth.Domain.Entities;

namespace MyApi.Modules.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IAuthUserStore _store;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(IAuthUserStore store, IPasswordHasher hasher, IJwtTokenGenerator jwt)
    {
        _store = store;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, string ipAddress = "")
    {
        var user = await _store.GetByEmailAsync(request.Email);
        if (user == null)
            throw new InvalidOperationException("Invalid credentials");

        if (!_hasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new InvalidOperationException("Invalid credentials");

        await _store.EnsureDefaultRoleAssignedAsync(user.Id);

        var roles = await _store.GetUserRolesAsync(user.Id);
        var permissions = await _store.GetUserPermissionsAsync(user.Id);

        var access = _jwt.GenerateAccessToken(user, roles, permissions);
        var refresh = _jwt.GenerateRefreshToken(user, ipAddress);

        await _store.AddRefreshTokenAsync(refresh);

        user.LastLoginAt = DateTime.UtcNow;
        await _store.UpdateAsync(user);

        return new LoginResponseDTO
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = access,
            RefreshToken = refresh.Token,
            TokenExpiresAt = DateTime.UtcNow.AddMinutes(60),
            Roles = roles,
            Permissions = permissions
        };
    }

    public async Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO request, string ipAddress = "")
    {
        var existing = await _store.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Email already registered");

        var (passwordHash, passwordSalt) = _hasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _store.AddAsync(user);
        await _store.EnsureDefaultRoleAssignedAsync(user.Id);

        var roles = await _store.GetUserRolesAsync(user.Id);
        var permissions = await _store.GetUserPermissionsAsync(user.Id);

        var access = _jwt.GenerateAccessToken(user, roles, permissions);
        var refresh = _jwt.GenerateRefreshToken(user, ipAddress);
        await _store.AddRefreshTokenAsync(refresh);

        return new LoginResponseDTO
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = access,
            RefreshToken = refresh.Token,
            TokenExpiresAt = refresh.ExpiryDate,
            Roles = roles,
            Permissions = permissions
        };
    }

    public async Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken, string ipAddress = "")
    {
        var tokenEntity = await _store.GetRefreshTokenAsync(refreshToken);
        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiryDate < DateTime.UtcNow)
            throw new InvalidOperationException("Invalid or expired refresh token");

        var user = await _store.GetByIdAsync(tokenEntity.UserId);
        if (user == null || !user.IsActive)
            throw new InvalidOperationException("User no longer exists or is inactive");

        await _store.EnsureDefaultRoleAssignedAsync(user.Id);

        var roles = await _store.GetUserRolesAsync(user.Id);
        var permissions = await _store.GetUserPermissionsAsync(user.Id);

        await _store.RevokeRefreshTokenAsync(tokenEntity);

        var newAccess = _jwt.GenerateAccessToken(user, roles, permissions);
        var newRefresh = _jwt.GenerateRefreshToken(user, ipAddress);
        await _store.AddRefreshTokenAsync(newRefresh);

        return new TokenResponseDTO
        {
            AccessToken = newAccess,
            RefreshToken = newRefresh.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            Roles = roles,
            Permissions = permissions
        };
    }
}
