using Microsoft.EntityFrameworkCore;
using ModuleDrivenFramwork.Modules.AccessControl.Application.DTOs;
using ModuleDrivenFramwork.Modules.AccessControl.Domain.Entities;
using ModuleDrivenFramwork.Modules.AccessControl.Persistence;

namespace ModuleDrivenFramwork.Modules.AccessControl.Services;

public class AccessControlService : IAccessControlService
{
    private readonly AccessControlDbContext _dbContext;

    public AccessControlService(AccessControlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SecurityContextDto> GetSecurityContextAsync(IEnumerable<Guid> roleIds)
    {
        var roleGuids = roleIds.ToList();
        var roles = await _dbContext.Roles
            .Where(r => roleGuids.Contains(r.Id))
            .ToListAsync();

        var roleNames = roles.Select(r => r.Name).ToList();
        
        // Full pass logic for Admin/SuperAdmin
        if (roleNames.Contains(AccessControlSeedData.AdminRoleName) || roleNames.Contains(AccessControlSeedData.SuperAdminRoleName))
        {
            var allPermissions = await _dbContext.Permissions
                .Select(p => p.Name)
                .ToListAsync();
            return new SecurityContextDto(roleNames, allPermissions);
        }

        var permissions = await _dbContext.RolePermissions
            .Where(rp => roleGuids.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync();

        return new SecurityContextDto(roleNames, permissions);
    }

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync()
    {
        var roles = await _dbContext.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .OrderBy(r => r.Name)
            .ToListAsync();
        return roles.Select(MapRole).ToList();
    }

    public async Task<RoleDto?> GetRoleAsync(Guid roleId)
    {
        var role = await _dbContext.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId);
        return role == null ? null : MapRole(role);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request)
    {
        var name = request.Name.Trim();
        if (await _dbContext.Roles.AnyAsync(r => r.Name == name))
            throw new InvalidOperationException("Role name already exists.");

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = request.Description.Trim(),
            RolePermissions = request.PermissionIds.Select(pId => new RolePermission
            {
                Id = Guid.NewGuid(),
                PermissionId = pId
            }).ToList()
        };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();
        return MapRole(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(Guid roleId, UpdateRoleRequestDto request)
    {
        var role = await _dbContext.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId)
            ?? throw new InvalidOperationException("Role not found.");

        var name = request.Name.Trim();
        if (await _dbContext.Roles.AnyAsync(r => r.Name == name && r.Id != roleId))
            throw new InvalidOperationException("Role name already exists.");

        role.Name = name;
        role.Description = request.Description.Trim();

        _dbContext.RolePermissions.RemoveRange(role.RolePermissions);
        role.RolePermissions = request.PermissionIds.Select(pId => new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = role.Id,
            PermissionId = pId
        }).ToList();

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("The role record has been modified or deleted by another user.");
        }

        return MapRole(role);
    }

    public async Task<DeleteResult> DeleteRoleAsync(Guid roleId)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId)
            ?? throw new InvalidOperationException("Role not found.");

        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync();
        return new DeleteResult(true);
    }

    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync()
    {
        return await _dbContext.Permissions
            .OrderBy(p => p.Name)
            .Select(p => new PermissionDto { Id = p.Id, Name = p.Name, Description = p.Description })
            .ToListAsync();
    }

    public async Task<PermissionDto?> GetPermissionAsync(Guid permissionId)
    {
        return await _dbContext.Permissions
            .Where(p => p.Id == permissionId)
            .Select(p => new PermissionDto { Id = p.Id, Name = p.Name, Description = p.Description })
            .FirstOrDefaultAsync();
    }

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionRequestDto request)
    {
        var name = request.Name.Trim();
        if (await _dbContext.Permissions.AnyAsync(p => p.Name == name))
            throw new InvalidOperationException("Permission name already exists.");

        var permission = new Permission { Id = Guid.NewGuid(), Name = name, Description = request.Description.Trim() };
        _dbContext.Permissions.Add(permission);
        await _dbContext.SaveChangesAsync();
        
        return new PermissionDto { Id = permission.Id, Name = permission.Name, Description = permission.Description };
    }

    public async Task<PermissionDto> UpdatePermissionAsync(Guid permissionId, UpdatePermissionRequestDto request)
    {
        var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId)
            ?? throw new InvalidOperationException("Permission not found.");

        var name = request.Name.Trim();
        if (await _dbContext.Permissions.AnyAsync(p => p.Name == name && p.Id != permissionId))
            throw new InvalidOperationException("Permission name already exists.");

        permission.Name = name;
        permission.Description = request.Description.Trim();

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("The permission record has been modified or deleted by another user.");
        }

        return new PermissionDto { Id = permission.Id, Name = permission.Name, Description = permission.Description };
    }

    public async Task DeletePermissionAsync(Guid permissionId)
    {
        var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId)
            ?? throw new InvalidOperationException("Permission not found.");

        _dbContext.Permissions.Remove(permission);
        await _dbContext.SaveChangesAsync();
    }

    private static RoleDto MapRole(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = role.RolePermissions
                .Select(rp => rp.Permission)
                .Where(p => p != null)
                .Select(p => new PermissionAssignmentDto { Id = p!.Id, Name = p!.Name })
                .ToList()
        };
    }
}
