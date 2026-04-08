namespace MyApi.Modules.Auth.Application.DTOs.AuthManagement;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IReadOnlyList<PermissionAssignmentDto> Permissions { get; set; } = [];
}

public class PermissionAssignmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IReadOnlyList<Guid> PermissionIds { get; set; } = [];
}

public class UpdateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IReadOnlyList<Guid> PermissionIds { get; set; } = [];
}