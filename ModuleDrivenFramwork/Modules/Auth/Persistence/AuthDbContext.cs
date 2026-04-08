using Microsoft.EntityFrameworkCore;
using ModuleDrivenFramwork.Modules.Auth.Domain.Entities;

namespace ModuleDrivenFramwork.Modules.Auth.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("AuthUsers");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(user => user.LastName).IsRequired().HasMaxLength(100);
            entity.Property(user => user.Email).IsRequired().HasMaxLength(255);
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.Property(user => user.PasswordSalt).IsRequired();
            entity.HasIndex(user => user.Email).IsUnique();
            entity.HasMany(user => user.RefreshTokens)
                .WithOne(token => token.User)
                .HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(user => user.UserRoles)
                .WithOne(userRole => userRole.User)
                .HasForeignKey(userRole => userRole.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("AuthRefreshTokens");
            entity.HasKey(token => token.Id);
            entity.Property(token => token.Token).IsRequired().HasMaxLength(512);
            entity.HasIndex(token => token.Token).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("AuthRoles");
            entity.HasKey(role => role.Id);
            entity.Property(role => role.Name).IsRequired().HasMaxLength(100);
            entity.Property(role => role.Description).IsRequired().HasMaxLength(500);
            entity.HasIndex(role => role.Name).IsUnique();
            entity.HasData(AuthSeedData.Roles);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("AuthPermissions");
            entity.HasKey(permission => permission.Id);
            entity.Property(permission => permission.Name).IsRequired().HasMaxLength(150);
            entity.Property(permission => permission.Description).IsRequired().HasMaxLength(500);
            entity.HasIndex(permission => permission.Name).IsUnique();
            entity.HasData(AuthSeedData.Permissions);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("AuthUserRoles");
            entity.HasKey(userRole => userRole.Id);
            entity.HasIndex(userRole => new { userRole.UserId, userRole.RoleId }).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("AuthRolePermissions");
            entity.HasKey(rolePermission => rolePermission.Id);
            entity.HasIndex(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId }).IsUnique();
            entity.HasData(AuthSeedData.RolePermissions);
        });
    }
}