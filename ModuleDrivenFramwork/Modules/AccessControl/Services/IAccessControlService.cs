using ModuleDrivenFramwork.Modules.AccessControl.Application.DTOs;

namespace ModuleDrivenFramwork.Modules.AccessControl.Services;

public interface IAccessControlService
{
    // Security Context
    Task<SecurityContextDto> GetSecurityContextAsync(IEnumerable<Guid> roleIds);

    // Roles
    Task<IReadOnlyList<RoleDto>> GetRolesAsync();
    Task<RoleDto?> GetRoleAsync(Guid roleId);
    Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request);
    Task<RoleDto> UpdateRoleAsync(Guid roleId, UpdateRoleRequestDto request);
    Task<DeleteResult> DeleteRoleAsync(Guid roleId);

    // Permissions
    Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync();
    Task<PermissionDto?> GetPermissionAsync(Guid permissionId);
    Task<PermissionDto> CreatePermissionAsync(CreatePermissionRequestDto request);
    Task<PermissionDto> UpdatePermissionAsync(Guid permissionId, UpdatePermissionRequestDto request);
    Task DeletePermissionAsync(Guid permissionId);
}

public record SecurityContextDto(List<string> Roles, List<string> Permissions);
public record DeleteResult(bool Success, string Message = "");
