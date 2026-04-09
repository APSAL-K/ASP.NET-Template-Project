using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuleDrivenFramwork.Modules.Auth.Application.DTOs.AuthManagement;

namespace ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

public interface IAuthManagementService
{
    // Users
    Task<IReadOnlyList<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserAsync(Guid userId);
    Task<UserDto> CreateUserAsync(CreateUserRequestDto request);
    Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request);
    Task DeleteUserAsync(Guid userId);
}