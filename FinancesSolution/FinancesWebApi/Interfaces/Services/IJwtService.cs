using System.IdentityModel.Tokens.Jwt;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User;

namespace FinancesWebApi.Interfaces.Services;

public interface IJwtService
{
    AccessToken Generate(User? user, int deviceId);
    RefreshToken GenerateRefreshToken();
    JwtSecurityToken Verify(string jwt);
    string GenerateRandomToken();
}