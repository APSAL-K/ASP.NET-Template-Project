using ModuleDrivenFramwork.Modules.Auth.Domain.Entities;

namespace ModuleDrivenFramwork.Modules.Auth.Persistence;

public static class AuthSeedData
{
    public const string UserRoleName = "User";
    public const string AdminRoleName = "Admin";
    public const string SuperAdminRoleName = "SuperAdmin";

    private static readonly DateTime SeededAtUtc = new(2026, 4, 7, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid UserRoleId = Guid.Parse("0c6b3d90-5513-4c67-9d7f-5bc2b4e2d9c1");
    public static readonly Guid AdminRoleId = Guid.Parse("0e2714f8-a22d-4ec3-a730-0710bf3efc4c");
    public static readonly Guid SuperAdminRoleId = Guid.Parse("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d");
}