using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyApi.Modules.Auth.Domain.Entities;

namespace MyApi.Modules.Auth.Application.Interfaces;

public interface IAuthUserStore
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid userId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task EnsureDefaultRoleAssignedAsync(Guid userId);
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task AddRefreshTokenAsync(RefreshToken token);
    Task RevokeRefreshTokenAsync(RefreshToken token);
}
