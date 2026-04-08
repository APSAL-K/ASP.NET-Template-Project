using MyApi.Modules.Auth.Application.DTOs.AuthManagement;

namespace MyApi.Modules.Auth.Application.Interfaces;

public interface IAuthManagementService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserAsync(Guid userId);
    Task<UserDto> CreateUserAsync(CreateUserRequestDto request);
    Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request);
    Task DeleteUserAsync(Guid userId);
    Task<IReadOnlyList<RoleDto>> GetRolesAsync();
    Task<RoleDto?> GetRoleAsync(Guid roleId);
    Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request);
    Task<RoleDto> UpdateRoleAsync(Guid roleId, UpdateRoleRequestDto request);
    Task DeleteRoleAsync(Guid roleId);
    Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync();
    Task<PermissionDto?> GetPermissionAsync(Guid permissionId);
    Task<PermissionDto> CreatePermissionAsync(CreatePermissionRequestDto request);
    Task<PermissionDto> UpdatePermissionAsync(Guid permissionId, UpdatePermissionRequestDto request);
    Task DeletePermissionAsync(Guid permissionId);
}