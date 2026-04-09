using ModuleDrivenFramwork.Modules.AccessControl.Domain.Entities;

namespace ModuleDrivenFramwork.Modules.AccessControl.Persistence;

public static class AccessControlSeedData
{
    public const string UserRoleName = "User";
    public const string AdminRoleName = "Admin";
    public const string SuperAdminRoleName = "SuperAdmin";

    public static readonly Guid UserRoleId = Guid.Parse("0c6b3d90-5513-4c67-9d7f-5bc2b4e2d9c1");
    public static readonly Guid AdminRoleId = Guid.Parse("0e2714f8-a22d-4ec3-a730-0710bf3efc4c");
    public static readonly Guid SuperAdminRoleId = Guid.Parse("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d");
    
    public static readonly Guid AuthProfileReadPermissionId = Guid.Parse("ccf425d8-c99e-4d54-b5f0-1949920727f0");
    public static readonly Guid AuthProfileUpdatePermissionId = Guid.Parse("11a7d0ca-a7aa-49d2-81c2-eb09fbf67e95");
    public static readonly Guid AuthTokensRefreshPermissionId = Guid.Parse("b1daf2a5-3508-4b35-a449-38590f266f59");
    public static readonly Guid AuthUsersManagePermissionId = Guid.Parse("9007725f-8a40-4a4c-9cdb-01e983f7f98d");

    public static readonly Role[] Roles =
    [
        new Role { Id = UserRoleId, Name = UserRoleName, Description = "Default application user role." },
        new Role { Id = AdminRoleId, Name = AdminRoleName, Description = "Administrator role with access to auth management operations." },
        new Role { Id = SuperAdminRoleId, Name = SuperAdminRoleName, Description = "System administrator role with unconditional system-wide access to all operations." }
    ];

    public static readonly Permission[] Permissions =
    [
        new Permission { Id = AuthProfileReadPermissionId, Name = "auth.profile.read", Description = "Read the authenticated user profile." },
        new Permission { Id = AuthProfileUpdatePermissionId, Name = "auth.profile.update", Description = "Update the authenticated user profile." },
        new Permission { Id = AuthTokensRefreshPermissionId, Name = "auth.tokens.refresh", Description = "Refresh access tokens." },
        new Permission { Id = AuthUsersManagePermissionId, Name = "auth.users.manage", Description = "Manage users, roles, and permissions." }
    ];

    public static readonly RolePermission[] RolePermissions =
    [
        new RolePermission { Id = Guid.NewGuid(), RoleId = UserRoleId, PermissionId = AuthProfileReadPermissionId },
        new RolePermission { Id = Guid.NewGuid(), RoleId = UserRoleId, PermissionId = AuthProfileUpdatePermissionId },
        new RolePermission { Id = Guid.NewGuid(), RoleId = UserRoleId, PermissionId = AuthTokensRefreshPermissionId },
        new RolePermission { Id = Guid.NewGuid(), RoleId = AdminRoleId, PermissionId = AuthProfileReadPermissionId },
        new RolePermission { Id = Guid.NewGuid(), RoleId = AdminRoleId, PermissionId = AuthProfileUpdatePermissionId },
        new RolePermission { Id = Guid.NewGuid(), RoleId = AdminRoleId, PermissionId = AuthTokensRefreshPermissionId },
        new RolePermission { Id = Guid.NewGuid(), RoleId = AdminRoleId, PermissionId = AuthUsersManagePermissionId }
    ];
}
