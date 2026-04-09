namespace ModuleDrivenFramwork.Modules.AccessControl.Application.DTOs;

public record RoleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<PermissionAssignmentDto> Permissions { get; init; } = new();
}

public record PermissionAssignmentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public record CreateRoleRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<Guid> PermissionIds { get; init; } = new();
}

public record UpdateRoleRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<Guid> PermissionIds { get; init; } = new();
}
