using ModuleDrivenFramwork.Common.Models;
using ModuleDrivenFramwork.Modules.Auth.Application.DTOs.AuthManagement;

namespace ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

public interface IAuthManagementService
{
    // Users
    Task<PaginatedResult<UserDto>> GetUsersAsync(int pageNumber, int pageSize);
    Task<UserDto?> GetUserAsync(Guid userId);
    Task<UserDto> CreateUserAsync(CreateUserRequestDto request);
    Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request);
    Task DeleteUserAsync(Guid userId);
}