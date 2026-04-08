using MyApi.Modules.Auth.Domain.Entities;

namespace MyApi.Modules.Auth.Persistence;

public static class AuthSeedData
{
    public const string UserRoleName = "User";
    public const string AdminRoleName = "Admin";

    private static readonly DateTime SeededAtUtc = new(2026, 4, 7, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid UserRoleId = Guid.Parse("0c6b3d90-5513-4c67-9d7f-5bc2b4e2d9c1");
    public static readonly Guid AdminRoleId = Guid.Parse("0e2714f8-a22d-4ec3-a730-0710bf3efc4c");
    public static readonly Guid AuthProfileReadPermissionId = Guid.Parse("ccf425d8-c99e-4d54-b5f0-1949920727f0");
    public static readonly Guid AuthProfileUpdatePermissionId = Guid.Parse("11a7d0ca-a7aa-49d2-81c2-eb09fbf67e95");
    public static readonly Guid AuthTokensRefreshPermissionId = Guid.Parse("b1daf2a5-3508-4b35-a449-38590f266f59");
    public static readonly Guid AuthUsersManagePermissionId = Guid.Parse("9007725f-8a40-4a4c-9cdb-01e983f7f98d");

    public static readonly Role[] Roles =
    [
        new Role
        {
            Id = UserRoleId,
            Name = UserRoleName,
            Description = "Default application user role."
        },
        new Role
        {
            Id = AdminRoleId,
            Name = AdminRoleName,
            Description = "Administrator role with access to auth management operations."
        }
    ];

    public static readonly Permission[] Permissions =
    [
        new Permission
        {
            Id = AuthProfileReadPermissionId,
            Name = "auth.profile.read",
            Description = "Read the authenticated user profile."
        },
        new Permission
        {
            Id = AuthProfileUpdatePermissionId,
            Name = "auth.profile.update",
            Description = "Update the authenticated user profile."
        },
        new Permission
        {
            Id = AuthTokensRefreshPermissionId,
            Name = "auth.tokens.refresh",
            Description = "Refresh access tokens."
        },
        new Permission
        {
            Id = AuthUsersManagePermissionId,
            Name = "auth.users.manage",
            Description = "Manage users, roles, and permissions."
        }
    ];

    public static readonly RolePermission[] RolePermissions =
    [
        new RolePermission
        {
            Id = Guid.Parse("21a9dc48-7e7b-4af5-89b8-c792d2113348"),
            RoleId = UserRoleId,
            PermissionId = AuthProfileReadPermissionId
        },
        new RolePermission
        {
            Id = Guid.Parse("6851ee5d-703e-41ef-aaad-071761761ce2"),
            RoleId = UserRoleId,
            PermissionId = AuthProfileUpdatePermissionId
        },
        new RolePermission
        {
            Id = Guid.Parse("f477c9b1-ac9a-4a1d-ab88-bd9302c970f1"),
            RoleId = UserRoleId,
            PermissionId = AuthTokensRefreshPermissionId
        },
        new RolePermission
        {
            Id = Guid.Parse("5ba40513-f48e-4671-9327-23344dacac51"),
            RoleId = AdminRoleId,
            PermissionId = AuthProfileReadPermissionId
        },
        new RolePermission
        {
            Id = Guid.Parse("f6560fd2-81b7-4673-ad0d-fc82ef3222db"),
            RoleId = AdminRoleId,
            PermissionId = AuthProfileUpdatePermissionId
        },
        new RolePermission
        {
            Id = Guid.Parse("9d23d6d5-b013-41a4-b9bb-430f8dd2e74d"),
            RoleId = AdminRoleId,
            PermissionId = AuthTokensRefreshPermissionId
        },
        new RolePermission
        {
            Id = Guid.Parse("77832da3-a93e-4d8b-b18b-31de7476c959"),
            RoleId = AdminRoleId,
            PermissionId = AuthUsersManagePermissionId
        }
    ];
}