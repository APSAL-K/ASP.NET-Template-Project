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

        // We use a hardcoded Guid for the default User role to avoid dependency on Role names in the store
        var userRoleId = Guid.Parse("0c6b3d90-5513-4c67-9d7f-5bc2b4e2d9c1");

        _dbContext.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = userRoleId,
            AssignedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Guid>> GetUserRoleIdsAsync(Guid userId)
    {
        return await _dbContext.UserRoles
            .Where(userRole => userRole.UserId == userId)
            .Select(userRole => userRole.RoleId)
            .Distinct()
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
