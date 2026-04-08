using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;
using ModuleDrivenFramwork.Modules.Auth.Domain.Entities;
using ModuleDrivenFramwork.Modules.Auth.Persistence;

namespace ModuleDrivenFramwork.Modules.Auth.Services;

public class AuthUserStore : IAuthUserStore
{
    private readonly AuthDbContext _dbContext;

    public AuthUserStore(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(User user)
    {
        _dbContext.Users.Add(user);
        return _dbContext.SaveChangesAsync();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public Task<User?> GetByIdAsync(Guid userId)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        return _dbContext.SaveChangesAsync();
    }

    public async Task EnsureDefaultRoleAssignedAsync(Guid userId)
    {
        var hasRole = await _dbContext.UserRoles.AnyAsync(userRole => userRole.UserId == userId);
        if (hasRole)
        {
            return;
        }

        var defaultRole = await _dbContext.Roles.FirstAsync(role => role.Name == AuthSeedData.UserRoleName);
        _dbContext.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = defaultRole.Id,
            AssignedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        return await _dbContext.UserRoles
            .Where(userRole => userRole.UserId == userId)
            .Select(userRole => userRole.Role.Name)
            .Distinct()
            .OrderBy(roleName => roleName)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId)
    {
        return await _dbContext.UserRoles
            .Where(userRole => userRole.UserId == userId)
            .SelectMany(userRole => userRole.Role.RolePermissions.Select(rolePermission => rolePermission.Permission.Name))
            .Distinct()
            .OrderBy(permissionName => permissionName)
            .ToListAsync();
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
    }

    public Task AddRefreshTokenAsync(RefreshToken token)
    {
        _dbContext.RefreshTokens.Add(token);
        return _dbContext.SaveChangesAsync();
    }

    public Task RevokeRefreshTokenAsync(RefreshToken token)
    {
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        _dbContext.RefreshTokens.Update(token);
        return _dbContext.SaveChangesAsync();
    }
}
