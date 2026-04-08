# ASP.NET Core Auth Module - Complete Implementation Guide

---

## 📋 Phase 6: Authentication & Authorization Module (COMPLETE IMPLEMENTATION)

### 6.1 Create Auth Module: `YourApp.Modules.Auth`
**Location:** `src/Modules/Auth/`

---

## 🏗️ PART 1: DOMAIN LAYER (Entities)

### 1.1 Entity Structure

```
YourApp.Modules.Auth/Domain/
├── Entities/
│   ├── Role.cs
│   ├── Permission.cs
│   ├── RolePermission.cs
│   ├── UserRole.cs
│   ├── RefreshToken.cs
│   └── AuditLog.cs
├── Enums/
│   ├── RoleType.cs
│   ├── PermissionType.cs
│   └── AuditAction.cs
└── Specifications/
    ├── UserWithRolesSpecification.cs
    └── RoleWithPermissionsSpecification.cs
```

### 1.2 Role.cs Entity
```csharp
using YourApp.Core.Entities;

namespace YourApp.Modules.Auth.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } // "Admin", "Manager", "User", "Guest"
    public string Description { get; set; }
    public RoleType RoleType { get; set; }
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } // 1-5 for hierarchy
    
    // Navigation Properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
```

### 1.3 Permission.cs Entity
```csharp
using YourApp.Core.Entities;

namespace YourApp.Modules.Auth.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } // "Users.Create", "Products.Edit", "Orders.Delete"
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Module { get; set; } // "Users", "Products", "Orders"
    public PermissionType PermissionType { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
```

### 1.4 RolePermission.cs Entity (Junction Table)
```csharp
using YourApp.Core.Entities;

namespace YourApp.Modules.Auth.Domain.Entities;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public bool IsGranted { get; set; } = true;
    
    // Navigation Properties
    public virtual Role Role { get; set; }
    public virtual Permission Permission { get; set; }
}
```

### 1.5 UserRole.cs Entity (Junction Table)
```csharp
using YourApp.Core.Entities;

namespace YourApp.Modules.Auth.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? AssignedBy { get; set; } // Admin who assigned
    public string AssignmentReason { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}
```

### 1.6 RefreshToken.cs Entity
```csharp
using YourApp.Core.Entities;

namespace YourApp.Modules.Auth.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string RevokeReason { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; }
}
```

### 1.7 AuditLog.cs Entity
```csharp
using YourApp.Core.Entities;

namespace YourApp.Modules.Auth.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string Action { get; set; } // "Login", "Register", "RoleAssign"
    public AuditAction AuditAction { get; set; }
    public string EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string OldValues { get; set; } // JSON
    public string NewValues { get; set; } // JSON
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; }
}
```

### 1.8 Enums

**RoleType.cs**
```csharp
namespace YourApp.Modules.Auth.Domain.Enums;

public enum RoleType
{
    SuperAdmin = 1,
    Admin = 2,
    Manager = 3,
    User = 4,
    Guest = 5
}
```

**PermissionType.cs**
```csharp
namespace YourApp.Modules.Auth.Domain.Enums;

public enum PermissionType
{
    Create = 1,
    Read = 2,
    Update = 3,
    Delete = 4,
    Execute = 5
}
```

**AuditAction.cs**
```csharp
namespace YourApp.Modules.Auth.Domain.Enums;

public enum AuditAction
{
    Login = 1,
    Logout = 2,
    Register = 3,
    PasswordChange = 4,
    RoleAssign = 5,
    RoleRevoke = 6,
    PermissionGrant = 7,
    PermissionRevoke = 8,
    AccountLocked = 9,
    AccountUnlocked = 10,
    EmailVerified = 11,
    FailedLogin = 12
}
```

### 1.9 Update User.cs Entity
```csharp
using YourApp.Core.Entities;

namespace YourApp.Core.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int LoginAttempts { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedUntil { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
```

---

## 📤 PART 2: APPLICATION LAYER (DTOs)

### 2.1 DTOs Structure

```
YourApp.Modules.Auth/Application/
├── DTOs/
│   ├── Auth/
│   │   ├── LoginRequestDTO.cs
│   │   ├── LoginResponseDTO.cs
│   │   ├── RegisterRequestDTO.cs
│   │   ├── RefreshTokenRequestDTO.cs
│   │   ├── TokenResponseDTO.cs
│   │   └── ChangePasswordDTO.cs
│   ├── Role/
│   │   ├── RoleDTO.cs
│   │   ├── CreateRoleDTO.cs
│   │   ├── UpdateRoleDTO.cs
│   │   └── RoleWithPermissionsDTO.cs
│   ├── Permission/
│   │   ├── PermissionDTO.cs
│   │   ├── CreatePermissionDTO.cs
│   │   └── AssignPermissionDTO.cs
│   └── User/
│       ├── UserDetailDTO.cs
│       ├── UserWithRolesDTO.cs
│       ├── AssignRoleDTO.cs
│       └── UserAuditDTO.cs
```

### 2.2 Auth DTOs

**LoginRequestDTO.cs**
```csharp
namespace YourApp.Modules.Auth.Application.DTOs.Auth;

public class LoginRequestDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; } = false;
}
```

**LoginResponseDTO.cs**
```csharp
namespace YourApp.Modules.Auth.Application.DTOs.Auth;

public class LoginResponseDTO
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpiresAt { get; set; }
    public IEnumerable<string> Roles { get; set; }
    public IEnumerable<string> Permissions { get; set; }
}
```

**RegisterRequestDTO.cs**
```csharp
namespace YourApp.Modules.Auth.Application.DTOs.Auth;

public class RegisterRequestDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string PhoneNumber { get; set; }
}
```

**RefreshTokenRequestDTO.cs**
```csharp
namespace YourApp.Modules.Auth.Application.DTOs.Auth;

public class RefreshTokenRequestDTO
{
    public string RefreshToken { get; set; }
}
```

**TokenResponseDTO.cs**
```csharp
namespace YourApp.Modules.Auth.Application.DTOs.Auth;

public class TokenResponseDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
```

**ChangePasswordDTO.cs**
```csharp
namespace YourApp.Modules.Auth.Application.DTOs.Auth;

public class ChangePasswordDTO
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
```

### 2.3 Role DTOs

**RoleDTO.cs**
```csharp
using YourApp.Modules.Auth.Domain.Enums;

namespace YourApp.Modules.Auth.Application.DTOs.Role;

public class RoleDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public RoleType RoleType { get; set; }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
}
```

**CreateRoleDTO.cs**
```csharp
using YourApp.Modules.Auth.Domain.Enums;

namespace YourApp.Modules.Auth.Application.DTOs.Role;

public class CreateRoleDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public RoleType RoleType { get; set; }
    public int Priority { get; set; }
}
```

**UpdateRoleDTO.cs**
```csharp
namespace YourApp.Modules.Auth.Application.DTOs.Role;

public class UpdateRoleDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
}
```

**RoleWithPermissionsDTO.cs**
```csharp
using YourApp.Modules.Auth.Application.DTOs.Permission;

namespace YourApp.Modules.Auth.Application.DTOs.Role;

public class RoleWithPermissionsDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<PermissionDTO> Permissions { get; set; }
}
```

### 2.4 Permission DTOs

**PermissionDTO.cs**
```csharp
using YourApp.Modules.Auth.Domain.Enums;

namespace YourApp.Modules.Auth.Application.DTOs.Permission;

public class PermissionDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Module { get; set; }
    public PermissionType PermissionType { get; set; }
    public bool IsActive { get; set; }
}
```

**CreatePermissionDTO.cs**
```csharp
using YourApp.Modules.Auth.Domain.Enums;

namespace YourApp.Modules.Auth.Application.DTOs.Permission;

public class CreatePermissionDTO
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Module { get; set; }
    public PermissionType PermissionType { get; set; }
}
```

### 2.5 User DTOs

**UserDetailDTO.cs**
```csharp
namespace YourApp.Modules.Auth.Application.DTOs.User;

public class UserDetailDTO
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
```

**UserWithRolesDTO.cs**
```csharp
using YourApp.Modules.Auth.Application.DTOs.Permission;
using YourApp.Modules.Auth.Application.DTOs.Role;

namespace YourApp.Modules.Auth.Application.DTOs.User;

public class UserWithRolesDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IEnumerable<RoleDTO> Roles { get; set; }
    public IEnumerable<PermissionDTO> Permissions { get; set; }
}
```

**AssignRoleDTO.cs**
```csharp
namespace YourApp.Modules.Auth.Application.DTOs.User;

public class AssignRoleDTO
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string AssignmentReason { get; set; }
}
```

---

## 🔧 PART 3: APPLICATION LAYER (Services)

### 3.1 Service Interfaces Structure

```
YourApp.Modules.Auth/Application/
└── Interfaces/
    ├── IAuthService.cs
    ├── ITokenService.cs
    ├── IRoleService.cs
    ├── IPermissionService.cs
    ├── IPasswordHasher.cs
    ├── IJwtTokenGenerator.cs
    ├── IAuditLogger.cs
    └── ICurrentUserService.cs
```

### 3.2 IAuthService.cs
```csharp
using YourApp.Modules.Auth.Application.DTOs.Auth;

namespace YourApp.Modules.Auth.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, string ipAddress);
    Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO request, string ipAddress);
    Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task LogoutAsync(Guid userId);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDTO request);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserDetailDTO> GetCurrentUserAsync(Guid userId);
}
```

### 3.3 IJwtTokenGenerator.cs
```csharp
using System.Security.Claims;
using YourApp.Core.Entities;

namespace YourApp.Modules.Auth.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
    RefreshToken GenerateRefreshToken(Guid userId, string ipAddress);
    bool ValidateToken(string token);
    ClaimsPrincipal GetPrincipalFromToken(string token);
}
```

### 3.4 IPasswordHasher.cs
```csharp
namespace YourApp.Modules.Auth.Application.Interfaces;

public interface IPasswordHasher
{
    (string hash, string salt) HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
```

### 3.5 IRoleService.cs
```csharp
using YourApp.Modules.Auth.Application.DTOs.Role;

namespace YourApp.Modules.Auth.Application.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
    Task<RoleDTO> GetRoleByIdAsync(Guid roleId);
    Task<RoleWithPermissionsDTO> GetRoleWithPermissionsAsync(Guid roleId);
    Task<RoleDTO> CreateRoleAsync(CreateRoleDTO request);
    Task<RoleDTO> UpdateRoleAsync(Guid roleId, UpdateRoleDTO request);
    Task DeleteRoleAsync(Guid roleId);
}
```

### 3.6 IPermissionService.cs
```csharp
using YourApp.Modules.Auth.Application.DTOs.Permission;

namespace YourApp.Modules.Auth.Application.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionDTO>> GetAllPermissionsAsync();
    Task<IEnumerable<PermissionDTO>> GetPermissionsByRoleAsync(Guid roleId);
    Task<PermissionDTO> CreatePermissionAsync(CreatePermissionDTO request);
    Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
    Task RevokePermissionFromRoleAsync(Guid roleId, Guid permissionId);
}
```

### 3.7 AuthService.cs Implementation
```csharp
using YourApp.Modules.Auth.Application.DTOs.Auth;
using YourApp.Modules.Auth.Application.Interfaces;
using YourApp.Modules.Auth.Domain.Entities;
using YourApp.Core.Entities;
using YourApp.Core.Exceptions;
using YourApp.Infrastructure.Persistence;

namespace YourApp.Modules.Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenGenerator tokenGenerator,
        IPasswordHasher passwordHasher,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, string ipAddress)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                await _auditLogger.LogFailedLoginAsync(request.Email, ipAddress);
                throw new InvalidOperationException("Invalid email or password");
            }

            if (user.IsLocked && user.LockedUntil > DateTime.UtcNow)
                throw new InvalidOperationException("Account is locked");

            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);

            var accessToken = _tokenGenerator.GenerateAccessToken(user, roles, permissions);
            var refreshToken = _tokenGenerator.GenerateRefreshToken(user.Id, ipAddress);

            user.LastLoginAt = DateTime.UtcNow;
            user.LoginAttempts = 0;
            await _userRepository.UpdateAsync(user);

            await _auditLogger.LogAuditAsync(
                user.Id, 
                AuditAction.Login, 
                "User", 
                user.Id, 
                true, 
                ipAddress);

            await _unitOfWork.SaveChangesAsync();

            return new LoginResponseDTO
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                TokenExpiresAt = DateTime.UtcNow.AddHours(1),
                Roles = roles,
                Permissions = permissions
            };
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO request, string ipAddress)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email already registered");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var (hash, salt) = _passwordHasher.HashPassword(request.Password);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        await _userRepository.AddAsync(user);

        // Assign default User role
        var defaultRole = await _userRepository.GetRoleByNameAsync("User");
        if (defaultRole != null)
        {
            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = defaultRole.Id,
                AssignedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AssignRoleAsync(userRole);
        }

        await _auditLogger.LogAuditAsync(
            user.Id, 
            AuditAction.Register, 
            "User", 
            user.Id, 
            true, 
            ipAddress);

        await _unitOfWork.SaveChangesAsync();

        var roles = new List<string> { "User" };
        var permissions = new List<string>();
        var accessToken = _tokenGenerator.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _tokenGenerator.GenerateRefreshToken(user.Id, ipAddress);

        return new LoginResponseDTO
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            TokenExpiresAt = DateTime.UtcNow.AddHours(1),
            Roles = roles,
            Permissions = permissions
        };
    }

    public async Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var tokenEntity = await _userRepository.GetRefreshTokenAsync(refreshToken);
        
        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiryDate < DateTime.UtcNow)
            throw new InvalidOperationException("Invalid or expired refresh token");

        var user = await _userRepository.GetByIdAsync(tokenEntity.UserId);
        var roles = await _userRepository.GetUserRolesAsync(user.Id);
        var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);

        var newAccessToken = _tokenGenerator.GenerateAccessToken(user, roles, permissions);
        var newRefreshToken = _tokenGenerator.GenerateRefreshToken(user.Id, ipAddress);

        tokenEntity.IsRevoked = true;
        tokenEntity.RevokedAt = DateTime.UtcNow;
        tokenEntity.RevokeReason = "Token rotated";

        await _userRepository.UpdateRefreshTokenAsync(tokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new TokenResponseDTO
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }

    public async Task LogoutAsync(Guid userId)
    {
        var refreshTokens = await _userRepository.GetUserRefreshTokensAsync(userId);
        foreach (var token in refreshTokens.Where(t => !t.IsRevoked))
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokeReason = "User logged out";
            await _userRepository.UpdateRefreshTokenAsync(token);
        }

        await _auditLogger.LogAuditAsync(
            userId, 
            AuditAction.Logout, 
            "User", 
            userId, 
            true, 
            "");

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDTO request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect");

        var (hash, salt) = _passwordHasher.HashPassword(request.NewPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        await _userRepository.UpdateAsync(user);

        await _auditLogger.LogAuditAsync(
            userId, 
            AuditAction.PasswordChange, 
            "User", 
            userId, 
            true, 
            "");

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        return _tokenGenerator.ValidateToken(token);
    }

    public async Task<UserDetailDTO> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        return new UserDetailDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsEmailVerified = user.IsEmailVerified,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt
        };
    }
}
```

### 3.8 JwtTokenGenerator.cs Implementation
```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using YourApp.Core.Entities;
using YourApp.Modules.Auth.Domain.Entities;
using YourApp.Modules.Auth.Application.Interfaces;
using YourApp.Infrastructure.Options;

namespace YourApp.Modules.Auth.Application.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtTokenGenerator> _logger;

    public JwtTokenGenerator(
        IOptions<JwtSettings> jwtSettings,
        ILogger<JwtTokenGenerator> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public string GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim("FullName", $"{user.FirstName} {user.LastName}")
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("Permission", permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.AccessTokenExpirationHours),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(Guid userId, string ipAddress)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            IpAddress = ipAddress,
            IssuedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Token validation failed: {ex.Message}");
            return false;
        }
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        }, out SecurityToken validatedToken);

        return principal;
    }
}
```

### 3.9 PasswordHasher.cs Implementation (using BCrypt)
```csharp
using BC = BCrypt.Net.BCrypt;
using YourApp.Modules.Auth.Application.Interfaces;

namespace YourApp.Modules.Auth.Application.Services;

public class PasswordHasher : IPasswordHasher
{
    public (string hash, string salt) HashPassword(string password)
    {
        var salt = BC.GenerateSalt(12);
        var hash = BC.HashPassword(password, salt);
        return (hash, salt);
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BC.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
```

---

## 🎛️ PART 4: PERSISTENCE LAYER (Database Configurations)

### 4.1 Entity Configurations Structure

```
YourApp.Modules.Auth/Persistence/
├── Configurations/
│   ├── RoleConfiguration.cs
│   ├── PermissionConfiguration.cs
│   ├── RolePermissionConfiguration.cs
│   ├── UserRoleConfiguration.cs
│   ├── RefreshTokenConfiguration.cs
│   └── AuditLogConfiguration.cs
└── Repositories/
    ├── RoleRepository.cs
    ├── PermissionRepository.cs
    ├── RefreshTokenRepository.cs
    └── AuditLogRepository.cs
```

### 4.2 RoleConfiguration.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YourApp.Modules.Auth.Domain.Entities;
using YourApp.Modules.Auth.Domain.Enums;

namespace YourApp.Modules.Auth.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RolePermissions)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed default roles
        builder.HasData(
            new Role
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "SuperAdmin",
                Description = "Super Administrator with full access",
                RoleType = RoleType.SuperAdmin,
                IsActive = true,
                Priority = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Admin",
                Description = "Administrator with high-level access",
                RoleType = RoleType.Admin,
                IsActive = true,
                Priority = 2,
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Manager",
                Description = "Manager with moderate access",
                RoleType = RoleType.Manager,
                IsActive = true,
                Priority = 3,
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = "User",
                Description = "Standard user with limited access",
                RoleType = RoleType.User,
                IsActive = true,
                Priority = 4,
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                Name = "Guest",
                Description = "Guest with minimal access",
                RoleType = RoleType.Guest,
                IsActive = true,
                Priority = 5,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
```

### 4.3 PermissionConfiguration.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YourApp.Modules.Auth.Domain.Entities;

namespace YourApp.Modules.Auth.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Module)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.RolePermissions)
            .WithOne(x => x.Permission)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### 4.4 RolePermissionConfiguration.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YourApp.Modules.Auth.Domain.Entities;

namespace YourApp.Modules.Auth.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.RoleId, x.PermissionId }).IsUnique();
    }
}
```

### 4.5 UserRoleConfiguration.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YourApp.Modules.Auth.Domain.Entities;

namespace YourApp.Modules.Auth.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();
    }
}
```

### 4.6 RefreshTokenConfiguration.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YourApp.Modules.Auth.Domain.Entities;

namespace YourApp.Modules.Auth.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiryDate);
    }
}
```

### 4.7 AuditLogConfiguration.cs
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YourApp.Modules.Auth.Domain.Entities;

namespace YourApp.Modules.Auth.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityType)
            .HasMaxLength(100);

        builder.Property(x => x.OldValues)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(x => x.NewValues)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.HasOne(x => x.User)
            .WithMany(x => x.AuditLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.UserId, x.Timestamp });
        builder.HasIndex(x => x.EntityType);
        builder.HasIndex(x => x.AuditAction);
        builder.HasIndex(x => x.Timestamp);
    }
}
```

---

## 🎯 PART 5: PRESENTATION LAYER (Controllers)

### 5.1 Controllers Structure

```
YourApp.Modules.Auth/Presentation/
└── Controllers/
    ├── AuthController.cs
    ├── RolesController.cs
    ├── PermissionsController.cs
    └── AuditController.cs
```

### 5.2 AuthController.cs
```csharp
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourApp.Modules.Auth.Application.DTOs.Auth;
using YourApp.Modules.Auth.Application.DTOs.User;
using YourApp.Modules.Auth.Application.Interfaces;

namespace YourApp.Modules.Auth.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var result = await _authService.LoginAsync(request, ipAddress);
        return Ok(result);
    }

    /// <summary>
    /// Register new user account
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var result = await _authService.RegisterAsync(request, ipAddress);
        return CreatedAtAction(nameof(GetCurrentUser), result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress);
        return Ok(result);
    }

    /// <summary>
    /// Logout user and revoke tokens
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        await _authService.LogoutAsync(userId);
        return NoContent();
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        await _authService.ChangePasswordAsync(userId, request);
        return NoContent();
    }

    /// <summary>
    /// Get current authenticated user details
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var result = await _authService.GetCurrentUserAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    [HttpPost("validate-token")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateToken([FromHeader] string authorization)
    {
        var token = authorization?.Replace("Bearer ", "");
        var isValid = await _authService.ValidateTokenAsync(token);
        return Ok(new { isValid });
    }
}
```

### 5.3 RolesController.cs
```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourApp.Modules.Auth.Application.DTOs.Role;
using YourApp.Modules.Auth.Application.Interfaces;

namespace YourApp.Modules.Auth.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _roleService.GetAllRolesAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _roleService.GetRoleByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("{id}/permissions")]
    public async Task<IActionResult> GetWithPermissions(Guid id)
    {
        var result = await _roleService.GetRoleWithPermissionsAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDTO request)
    {
        var result = await _roleService.CreateRoleAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDTO request)
    {
        var result = await _roleService.UpdateRoleAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roleService.DeleteRoleAsync(id);
        return NoContent();
    }
}
```

### 5.4 PermissionsController.cs
```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourApp.Modules.Auth.Application.DTOs.Permission;
using YourApp.Modules.Auth.Application.Interfaces;

namespace YourApp.Modules.Auth.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _permissionService.GetAllPermissionsAsync();
        return Ok(result);
    }

    [HttpGet("role/{roleId}")]
    public async Task<IActionResult> GetByRole(Guid roleId)
    {
        var result = await _permissionService.GetPermissionsByRoleAsync(roleId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePermissionDTO request)
    {
        var result = await _permissionService.CreatePermissionAsync(request);
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpPost("role/{roleId}/assign/{permissionId}")]
    public async Task<IActionResult> AssignToRole(Guid roleId, Guid permissionId)
    {
        await _permissionService.AssignPermissionToRoleAsync(roleId, permissionId);
        return NoContent();
    }

    [HttpPost("role/{roleId}/revoke/{permissionId}")]
    public async Task<IActionResult> RevokeFromRole(Guid roleId, Guid permissionId)
    {
        await _permissionService.RevokePermissionFromRoleAsync(roleId, permissionId);
        return NoContent();
    }
}
```

### 5.5 AuditController.cs
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourApp.Modules.Auth.Application.Interfaces;

namespace YourApp.Modules.Auth.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class AuditController : ControllerBase
{
    private readonly IAuditLogger _auditLogger;

    public AuditController(IAuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAuditLogs(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _auditLogger.GetUserAuditLogsAsync(userId, pageNumber, pageSize);
        return Ok(result);
    }
}
```

---

## 📊 PART 6: DATABASE MIGRATIONS

### 6.1 Create Migration
```bash
cd YourApp.Infrastructure
Add-Migration InitializeAuthModule -Project YourApp.Infrastructure
```

### 6.2 SQL Schema

```sql
-- Roles Table
CREATE TABLE [dbo].[Roles] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(500),
    [RoleType] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [Priority] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2
);

CREATE INDEX [IX_Roles_Name] ON [dbo].[Roles]([Name]);

-- Permissions Table
CREATE TABLE [dbo].[Permissions] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [DisplayName] NVARCHAR(150) NOT NULL,
    [Description] NVARCHAR(500),
    [Module] NVARCHAR(50) NOT NULL,
    [PermissionType] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2
);

CREATE INDEX [IX_Permissions_Module] ON [dbo].[Permissions]([Module]);

-- RolePermissions Junction Table
CREATE TABLE [dbo].[RolePermissions] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [PermissionId] UNIQUEIDENTIFIER NOT NULL,
    [IsGranted] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2,
    CONSTRAINT [FK_RolePermissions_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolePermissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions]([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_RolePermission] UNIQUE ([RoleId], [PermissionId])
);

-- UserRoles Junction Table
CREATE TABLE [dbo].[UserRoles] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [AssignedAt] DATETIME2 NOT NULL,
    [AssignedBy] UNIQUEIDENTIFIER,
    [AssignmentReason] NVARCHAR(MAX),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2,
    CONSTRAINT [FK_UserRoles_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_UserRole] UNIQUE ([UserId], [RoleId])
);

-- RefreshTokens Table
CREATE TABLE [dbo].[RefreshTokens] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Token] NVARCHAR(500) NOT NULL UNIQUE,
    [ExpiryDate] DATETIME2 NOT NULL,
    [IssuedAt] DATETIME2 NOT NULL,
    [IpAddress] NVARCHAR(45),
    [UserAgent] NVARCHAR(500),
    [IsRevoked] BIT NOT NULL DEFAULT 0,
    [RevokedAt] DATETIME2,
    [RevokeReason] NVARCHAR(MAX),
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2,
    CONSTRAINT [FK_RefreshTokens_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_RefreshTokens_Token] ON [dbo].[RefreshTokens]([Token]);
CREATE INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens]([UserId]);
CREATE INDEX [IX_RefreshTokens_ExpiryDate] ON [dbo].[RefreshTokens]([ExpiryDate]);

-- AuditLogs Table
CREATE TABLE [dbo].[AuditLogs] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Action] NVARCHAR(100) NOT NULL,
    [AuditAction] INT NOT NULL,
    [EntityType] NVARCHAR(100),
    [EntityId] UNIQUEIDENTIFIER,
    [OldValues] NVARCHAR(MAX),
    [NewValues] NVARCHAR(MAX),
    [IpAddress] NVARCHAR(45),
    [UserAgent] NVARCHAR(500),
    [Timestamp] DATETIME2 NOT NULL,
    [IsSuccess] BIT NOT NULL,
    [ErrorMessage] NVARCHAR(MAX),
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2,
    CONSTRAINT [FK_AuditLogs_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
);

CREATE INDEX [IX_AuditLogs_UserId_Timestamp] ON [dbo].[AuditLogs]([UserId], [Timestamp]);
CREATE INDEX [IX_AuditLogs_EntityType] ON [dbo].[AuditLogs]([EntityType]);
CREATE INDEX [IX_AuditLogs_AuditAction] ON [dbo].[AuditLogs]([AuditAction]);
CREATE INDEX [IX_AuditLogs_Timestamp] ON [dbo].[AuditLogs]([Timestamp]);
```

---

## ⚙️ PART 7: CONFIGURATION

### 7.1 appsettings.json
```json
{
  "JwtSettings": {
    "Secret": "YourVeryLongSecretKeyThatIsAtLeast32CharactersLong12345",
    "Issuer": "YourApp",
    "Audience": "YourAppUsers",
    "AccessTokenExpirationHours": 1,
    "RefreshTokenExpirationDays": 7
  },
  "PasswordSettings": {
    "RequireDigit": true,
    "RequireLowercase": true,
    "RequireUppercase": true,
    "RequireSpecialChar": true,
    "MinimumLength": 8
  },
  "SecuritySettings": {
    "MaxLoginAttempts": 5,
    "LockoutMinutes": 15
  }
}
```

### 7.2 Program.cs Registration
```csharp
// Install NuGet packages
// dotnet add package System.IdentityModel.Tokens.Jwt
// dotnet add package BCrypt.Net-Next
// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

// Add Auth Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();

// Configure JWT Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
```

---

## 🔄 PART 8: AUTHENTICATION FLOW DIAGRAM

```
┌─────────────────────────────────────────────────────┐
│                  LOGIN REQUEST                       │
├─────────────────────────────────────────────────────┤
│ POST /api/auth/login                                │
│ {email: "user@example.com", password: "pass123"}    │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│         IAuthService.LoginAsync()                   │
├─────────────────────────────────────────────────────┤
│ 1. Find user by email                              │
│ 2. Verify password with BCrypt                     │
│ 3. Check if account locked                         │
│ 4. Get user roles and permissions                  │
│ 5. Generate JWT access token (1hr)                 │
│ 6. Generate refresh token (7d)                     │
│ 7. Update last login timestamp                     │
│ 8. Log audit event (Login)                         │
│ 9. Save to database                                │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│           LOGIN RESPONSE                             │
├─────────────────────────────────────────────────────┤
│ {                                                    │
│   accessToken: "eyJhbGc...",                        │
│   refreshToken: "base64_encoded_token",             │
│   expiresAt: "2024-01-01T15:00:00Z",               │
│   roles: ["User"],                                  │
│   permissions: []                                   │
│ }                                                    │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│         CLIENT STORES TOKENS                         │
├─────────────────────────────────────────────────────┤
│ AccessToken → Memory/SessionStorage (expires 1hr)  │
│ RefreshToken → HttpOnly Cookie (expires 7d)        │
└─────────────────────────────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
        ▼                     ▼
┌─────────────────┐   ┌──────────────────────┐
│  PROTECTED API  │   │ TOKEN EXPIRY (1 hour)│
│  REQUEST        │   │ POST /api/auth/      │
│                 │   │ refresh-token        │
│ GET /api/users/ │   │                      │
│ Authorization:  │   │ {refreshToken: ".."}│
│ Bearer {token}  │   └──────────────────────┘
└────────┬────────┘              │
         │                       ▼
         │              ┌──────────────────────┐
         │              │   VALIDATE TOKEN     │
         ▼              │ Check if revoked     │
    ┌─────────┐         │ Check expiry         │
    │ Validate│         │ Generate new token   │
    │ JWT     │         └──────────┬───────────┘
    └─────┬───┘                    │
          │                        ▼
          │              ┌──────────────────────┐
          │              │ NEW TOKENS RESPONSE  │
          │              │ AccessToken (1hr)    │
          │              │ RefreshToken (7d)    │
          │              └──────────────────────┘
          ▼
    ┌─────────────┐
    │ Auth Header │
    │ Valid?      │
    └─────┬───────┘
          │
    ┌─────┴─────┐
    │           │
   YES         NO
    │           │
    ▼           ▼
  200 OK    401 Unauthorized
  Process   Redirect to login
  Request
```

---

## 🔒 PART 9: ROLES & PERMISSIONS HIERARCHY

### 9.1 Default Roles

| Role | Priority | Use Case | Default Permissions |
|------|----------|----------|-------------------|
| SuperAdmin | 1 | System Administrator | All |
| Admin | 2 | Administrator | All except System |
| Manager | 3 | Manage department | Create, Read, Update |
| User | 4 | Regular user | Create, Read |
| Guest | 5 | Public access | Read only |

### 9.2 Sample Permissions

**Users Module:**
- Users.Create
- Users.Read
- Users.Update
- Users.Delete
- Users.ManageRoles
- Users.Export

**Products Module:**
- Products.Create
- Products.Read
- Products.Update
- Products.Delete
- Products.Publish
- Products.Download

**Orders Module:**
- Orders.Create
- Orders.Read
- Orders.Update
- Orders.Cancel
- Orders.Export
- Orders.Refund

### 9.3 Role-Permission Matrix

```
SuperAdmin:  All permissions ✓
Admin:       Users.*, Products.*, Orders.* (except sensitive)
Manager:     Products.Create ✓, Products.Read ✓, Products.Update ✓
User:        Products.Read ✓, Orders.Create ✓, Orders.Read (own)
Guest:       Products.Read ✓
```

---

## ✅ Implementation Checklist

- [ ] Create all domain entities (Role, Permission, etc.)
- [ ] Create all DTOs
- [ ] Create validators
- [ ] Implement auth service
- [ ] Implement JWT token generator
- [ ] Implement password hasher with BCrypt
- [ ] Create role and permission services
- [ ] Create audit logger
- [ ] Create database configurations
- [ ] Create migration
- [ ] Create all controllers
- [ ] Register services in Program.cs
- [ ] Test login/register flow
- [ ] Test token refresh
- [ ] Test logout
- [ ] Test role-based authorization
- [ ] Test permission-based authorization
- [ ] Test audit logging
- [ ] Update Swagger/OpenAPI documentation

