using System.Collections.Generic;
using ModuleDrivenFramwork.Modules.Auth.Domain.Entities;

namespace ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
    RefreshToken GenerateRefreshToken(User user, string ipAddress);
    bool ValidateToken(string token);
}
