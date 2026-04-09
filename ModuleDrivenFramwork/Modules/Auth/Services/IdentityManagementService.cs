using Microsoft.EntityFrameworkCore;
using ModuleDrivenFramwork.Common.Models;
using ModuleDrivenFramwork.Modules.Auth.Application.DTOs.AuthManagement;
using ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;
using ModuleDrivenFramwork.Modules.Auth.Domain.Entities;
using ModuleDrivenFramwork.Modules.Auth.Persistence;
using ModuleDrivenFramwork.Modules.AccessControl.Services;

namespace ModuleDrivenFramwork.Modules.Auth.Services;

public class IdentityManagementService : IAuthManagementService
{
    private readonly AuthDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessControlService _accessControl;

    public IdentityManagementService(
        AuthDbContext dbContext, 
        IPasswordHasher passwordHasher,
        IAccessControlService accessControl)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _accessControl = accessControl;
    }

    public async Task<PaginatedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Users
            .Include(u => u.UserRoles)
            .OrderBy(u => u.Email);

        var total = await query.CountAsync();
        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new List<UserDto>();
        foreach (var user in users)
        {
            result.Add(await MapUserAsync(user));
        }
        
        return new PaginatedResult<UserDto>(result, total, pageNumber, pageSize);
    }

    public async Task<UserDto?> GetUserAsync(Guid userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user == null ? null : await MapUserAsync(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequestDto request)
    {
        var email = request.Email.Trim().ToLower();
        if (await _dbContext.Users.AnyAsync(u => u.Email == email))
            throw new InvalidOperationException("Email already exists.");

        var (hash, salt) = _passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UserRoles = request.RoleIds.Select(rId => new UserRole
            {
                Id = Guid.NewGuid(),
                RoleId = rId,
                AssignedAt = DateTime.UtcNow
            }).ToList()
        };

        foreach (var ur in user.UserRoles) ur.UserId = user.Id;

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return await MapUserAsync(user);
    }

    public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new InvalidOperationException("User not found.");

        var email = request.Email.Trim().ToLower();
        if (await _dbContext.Users.AnyAsync(u => u.Email == email && u.Id != userId))
            throw new InvalidOperationException("Email already exists.");

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = email;
        user.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var (hash, salt) = _passwordHasher.HashPassword(request.Password);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
        }

        _dbContext.UserRoles.RemoveRange(user.UserRoles);
        user.UserRoles = request.RoleIds.Select(rId => new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = rId,
            AssignedAt = DateTime.UtcNow
        }).ToList();

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("The user record has been modified or deleted by another user.");
        }

        return await MapUserAsync(user);
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new InvalidOperationException("User not found.");

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<UserDto> MapUserAsync(User user)
    {
        var roleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
        var securityContext = await _accessControl.GetSecurityContextAsync(roleIds);

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = securityContext.Roles.Select((name, index) => new RoleAssignmentDto
            {
                Id = roleIds[index], // This is a rough mapping, ideally GetSecurityContext would return name-id pairs
                Name = name
            }).ToList(),
            Permissions = securityContext.Permissions
        };
    }
}
