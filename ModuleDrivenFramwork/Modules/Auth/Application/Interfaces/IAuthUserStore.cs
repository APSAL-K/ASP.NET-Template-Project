using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuleDrivenFramwork.Modules.Auth.Domain.Entities;

namespace ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

public interface IAuthUserStore
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid userId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task EnsureDefaultRoleAssignedAsync(Guid userId);
    Task<IEnumerable<Guid>> GetUserRoleIdsAsync(Guid userId);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task AddRefreshTokenAsync(RefreshToken token);
    Task RevokeRefreshTokenAsync(RefreshToken token);
}
