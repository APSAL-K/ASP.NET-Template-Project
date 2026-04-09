using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ModuleDrivenFramwork.Modules.Auth.Application.DTOs.Auth;
using ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;
using ModuleDrivenFramwork.Modules.Auth.Domain.Entities;
using ModuleDrivenFramwork.Modules.AccessControl.Services;

namespace ModuleDrivenFramwork.Modules.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IAuthUserStore _store;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IAccessControlService _accessControl;

    public AuthService(
        IAuthUserStore store, 
        IPasswordHasher hasher, 
        IJwtTokenGenerator jwt,
        IAccessControlService accessControl)
    {
        _store = store;
        _hasher = hasher;
        _jwt = jwt;
        _accessControl = accessControl;
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, string ipAddress = "")
    {
        var user = await _store.GetByEmailAsync(request.Email);
        if (user == null)
            throw new InvalidOperationException("Invalid credentials");

        if (!_hasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new InvalidOperationException("Invalid credentials");

        await _store.EnsureDefaultRoleAssignedAsync(user.Id);

        var roleIds = await _store.GetUserRoleIdsAsync(user.Id);
        
        // Handle explicit role selection if requested
        if (request.RoleId.HasValue)
        {
            if (!roleIds.Contains(request.RoleId.Value))
                throw new InvalidOperationException("Unauthorized for the selected role.");
            
            roleIds = new List<Guid> { request.RoleId.Value };
        }

        var securityContext = await _accessControl.GetSecurityContextAsync(roleIds);

        var access = _jwt.GenerateAccessToken(user, securityContext.Roles, securityContext.Permissions);
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
            Roles = securityContext.Roles,
            Permissions = securityContext.Permissions
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
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLower(),
            CreatedAt = DateTime.UtcNow,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = true
        };

        try 
        {
            await _store.AddAsync(user);
            
            // Assign explicitly requested roles or default
            if (request.RoleIds != null && request.RoleIds.Any())
            {
                await _store.AssignRolesAsync(user.Id, request.RoleIds);
            }
            else
            {
                await _store.EnsureDefaultRoleAssignedAsync(user.Id);
            }

            var roleIds = await _store.GetUserRoleIdsAsync(user.Id);
            var securityContext = await _accessControl.GetSecurityContextAsync(roleIds);

            var access = _jwt.GenerateAccessToken(user, securityContext.Roles, securityContext.Permissions);
            var refresh = _jwt.GenerateRefreshToken(user, ipAddress);
            await _store.AddRefreshTokenAsync(refresh);

            return new LoginResponseDTO
            {
                UserId = user.Id,
                Email = user.Email,
                AccessToken = access,
                RefreshToken = refresh.Token,
                TokenExpiresAt = refresh.ExpiryDate,
                Roles = securityContext.Roles,
                Permissions = securityContext.Permissions
            };
        }
        catch (System.Exception ex)
        {
            throw new InvalidOperationException($"Registration failed during record creation: {ex.Message}", ex);
        }
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

        var roleIds = await _store.GetUserRoleIdsAsync(user.Id);
        var securityContext = await _accessControl.GetSecurityContextAsync(roleIds);

        await _store.RevokeRefreshTokenAsync(tokenEntity);

        var newAccess = _jwt.GenerateAccessToken(user, securityContext.Roles, securityContext.Permissions);
        var newRefresh = _jwt.GenerateRefreshToken(user, ipAddress);
        await _store.AddRefreshTokenAsync(newRefresh);

        return new TokenResponseDTO
        {
            AccessToken = newAccess,
            RefreshToken = newRefresh.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            Roles = securityContext.Roles,
            Permissions = securityContext.Permissions
        };
    }
}
