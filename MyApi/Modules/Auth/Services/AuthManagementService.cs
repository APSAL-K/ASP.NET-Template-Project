using Microsoft.EntityFrameworkCore;
using MyApi.Modules.Auth.Application.DTOs.AuthManagement;
using MyApi.Modules.Auth.Application.Interfaces;
using MyApi.Modules.Auth.Domain.Entities;
using MyApi.Modules.Auth.Persistence;

namespace MyApi.Modules.Auth.Services;

public class AuthManagementService : IAuthManagementService
{
    private readonly AuthDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public AuthManagementService(AuthDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync()
    {
        var users = await QueryUsers().ToListAsync();
        return users.Select(MapUser).ToList();
    }

    public async Task<UserDto?> GetUserAsync(Guid userId)
    {
        var user = await QueryUsers().FirstOrDefaultAsync(user => user.Id == userId);
        return user == null ? null : MapUser(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequestDto request)
    {
        var normalizedEmail = request.Email.Trim();
        await EnsureUniqueUserEmailAsync(normalizedEmail, null);

        var roleIds = request.RoleIds.Count == 0
            ? [AuthSeedData.UserRoleId]
            : request.RoleIds.Distinct().ToArray();

        var roles = await LoadRolesAsync(roleIds);
        var (hash, salt) = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UserRoles = roles.Select(role => new UserRole
            {
                UserId = Guid.Empty,
                RoleId = role.Id,
                Role = role,
                AssignedAt = DateTime.UtcNow
            }).ToList()
        };

        foreach (var userRole in user.UserRoles)
        {
            userRole.UserId = user.Id;
        }

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return await GetUserAsync(user.Id) ?? throw new InvalidOperationException("User was created but could not be reloaded.");
    }

    public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request)
    {
        var user = await _dbContext.Users
            .Include(existingUser => existingUser.UserRoles)
            .FirstOrDefaultAsync(existingUser => existingUser.Id == userId)
            ?? throw new InvalidOperationException("User not found.");

        var normalizedEmail = request.Email.Trim();
        await EnsureUniqueUserEmailAsync(normalizedEmail, userId);
        var roles = await LoadRolesAsync(request.RoleIds.Distinct().ToArray());

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = normalizedEmail;
        user.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var (hash, salt) = _passwordHasher.HashPassword(request.Password);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
        }

        _dbContext.UserRoles.RemoveRange(user.UserRoles);
        user.UserRoles = roles.Select(role => new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            AssignedAt = DateTime.UtcNow
        }).ToList();

        await _dbContext.SaveChangesAsync();

        return await GetUserAsync(user.Id) ?? throw new InvalidOperationException("User was updated but could not be reloaded.");
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(existingUser => existingUser.Id == userId)
            ?? throw new InvalidOperationException("User not found.");

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync()
    {
        var roles = await QueryRoles().ToListAsync();
        return roles.Select(MapRole).ToList();
    }

    public async Task<RoleDto?> GetRoleAsync(Guid roleId)
    {
        var role = await QueryRoles().FirstOrDefaultAsync(existingRole => existingRole.Id == roleId);
        return role == null ? null : MapRole(role);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request)
    {
        var normalizedName = request.Name.Trim();
        await EnsureUniqueRoleNameAsync(normalizedName, null);
        var permissions = await LoadPermissionsAsync(request.PermissionIds.Distinct().ToArray());

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Description = request.Description.Trim(),
            RolePermissions = permissions.Select(permission => new RolePermission
            {
                Id = Guid.NewGuid(),
                PermissionId = permission.Id
            }).ToList()
        };

        foreach (var rolePermission in role.RolePermissions)
        {
            rolePermission.RoleId = role.Id;
        }

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        return await GetRoleAsync(role.Id) ?? throw new InvalidOperationException("Role was created but could not be reloaded.");
    }

    public async Task<RoleDto> UpdateRoleAsync(Guid roleId, UpdateRoleRequestDto request)
    {
        var role = await _dbContext.Roles
            .Include(existingRole => existingRole.RolePermissions)
            .FirstOrDefaultAsync(existingRole => existingRole.Id == roleId)
            ?? throw new InvalidOperationException("Role not found.");

        var normalizedName = request.Name.Trim();
        await EnsureUniqueRoleNameAsync(normalizedName, roleId);
        var permissions = await LoadPermissionsAsync(request.PermissionIds.Distinct().ToArray());

        role.Name = normalizedName;
        role.Description = request.Description.Trim();

        _dbContext.RolePermissions.RemoveRange(role.RolePermissions);
        role.RolePermissions = permissions.Select(permission => new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = role.Id,
            PermissionId = permission.Id
        }).ToList();

        await _dbContext.SaveChangesAsync();

        return await GetRoleAsync(role.Id) ?? throw new InvalidOperationException("Role was updated but could not be reloaded.");
    }

    public async Task DeleteRoleAsync(Guid roleId)
    {
        var role = await _dbContext.Roles
            .Include(existingRole => existingRole.UserRoles)
            .FirstOrDefaultAsync(existingRole => existingRole.Id == roleId)
            ?? throw new InvalidOperationException("Role not found.");

        if (role.UserRoles.Count > 0)
        {
            throw new InvalidOperationException("Cannot delete a role that is assigned to users.");
        }

        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync()
    {
        return await _dbContext.Permissions
            .OrderBy(permission => permission.Name)
            .Select(permission => new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description
            })
            .ToListAsync();
    }

    public async Task<PermissionDto?> GetPermissionAsync(Guid permissionId)
    {
        return await _dbContext.Permissions
            .Where(permission => permission.Id == permissionId)
            .Select(permission => new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionRequestDto request)
    {
        var normalizedName = request.Name.Trim();
        await EnsureUniquePermissionNameAsync(normalizedName, null);

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Description = request.Description.Trim()
        };

        _dbContext.Permissions.Add(permission);
        await _dbContext.SaveChangesAsync();

        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description
        };
    }

    public async Task<PermissionDto> UpdatePermissionAsync(Guid permissionId, UpdatePermissionRequestDto request)
    {
        var permission = await _dbContext.Permissions.FirstOrDefaultAsync(existingPermission => existingPermission.Id == permissionId)
            ?? throw new InvalidOperationException("Permission not found.");

        var normalizedName = request.Name.Trim();
        await EnsureUniquePermissionNameAsync(normalizedName, permissionId);

        permission.Name = normalizedName;
        permission.Description = request.Description.Trim();
        await _dbContext.SaveChangesAsync();

        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description
        };
    }

    public async Task DeletePermissionAsync(Guid permissionId)
    {
        var permission = await _dbContext.Permissions.FirstOrDefaultAsync(existingPermission => existingPermission.Id == permissionId)
            ?? throw new InvalidOperationException("Permission not found.");

        _dbContext.Permissions.Remove(permission);
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<User> QueryUsers()
    {
        return _dbContext.Users
            .Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.Role)
            .Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.Role)
                .ThenInclude(role => role.RolePermissions)
                    .ThenInclude(rolePermission => rolePermission.Permission)
            .AsNoTracking()
            .OrderBy(user => user.Email);
    }

    private IQueryable<Role> QueryRoles()
    {
        return _dbContext.Roles
            .Include(role => role.RolePermissions)
                .ThenInclude(rolePermission => rolePermission.Permission)
            .AsNoTracking()
            .OrderBy(role => role.Name);
    }

    private async Task<List<Role>> LoadRolesAsync(IEnumerable<Guid> roleIds)
    {
        var ids = roleIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return [];
        }

        var roles = await _dbContext.Roles.Where(role => ids.Contains(role.Id)).ToListAsync();
        if (roles.Count != ids.Length)
        {
            throw new InvalidOperationException("One or more roles do not exist.");
        }

        return roles;
    }

    private async Task<List<Permission>> LoadPermissionsAsync(IEnumerable<Guid> permissionIds)
    {
        var ids = permissionIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return [];
        }

        var permissions = await _dbContext.Permissions.Where(permission => ids.Contains(permission.Id)).ToListAsync();
        if (permissions.Count != ids.Length)
        {
            throw new InvalidOperationException("One or more permissions do not exist.");
        }

        return permissions;
    }

    private async Task EnsureUniqueUserEmailAsync(string email, Guid? existingUserId)
    {
        var exists = await _dbContext.Users.AnyAsync(user => user.Email == email && user.Id != existingUserId);
        if (exists)
        {
            throw new InvalidOperationException("Email already exists.");
        }
    }

    private async Task EnsureUniqueRoleNameAsync(string name, Guid? existingRoleId)
    {
        var exists = await _dbContext.Roles.AnyAsync(role => role.Name == name && role.Id != existingRoleId);
        if (exists)
        {
            throw new InvalidOperationException("Role name already exists.");
        }
    }

    private async Task EnsureUniquePermissionNameAsync(string name, Guid? existingPermissionId)
    {
        var exists = await _dbContext.Permissions.AnyAsync(permission => permission.Name == name && permission.Id != existingPermissionId);
        if (exists)
        {
            throw new InvalidOperationException("Permission name already exists.");
        }
    }

    private static UserDto MapUser(User user)
    {
        var roles = user.UserRoles
            .Select(userRole => userRole.Role)
            .DistinctBy(role => role.Id)
            .OrderBy(role => role.Name)
            .ToList();

        var permissions = roles
            .SelectMany(role => role.RolePermissions)
            .Select(rolePermission => rolePermission.Permission.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(permissionName => permissionName)
            .ToList();

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles.Select(role => new RoleAssignmentDto
            {
                Id = role.Id,
                Name = role.Name
            }).ToList(),
            Permissions = permissions
        };
    }

    private static RoleDto MapRole(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = role.RolePermissions
                .Select(rolePermission => rolePermission.Permission)
                .DistinctBy(permission => permission.Id)
                .OrderBy(permission => permission.Name)
                .Select(permission => new PermissionAssignmentDto
                {
                    Id = permission.Id,
                    Name = permission.Name
                })
                .ToList()
        };
    }
}