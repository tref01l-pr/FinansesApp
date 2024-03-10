using System.IdentityModel.Tokens.Jwt;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User;

namespace FinancesWebApi.Interfaces.Services;

public interface IJwtService
{
    string Generate(User user);
    RefreshToken GenerateRefreshToken();
    JwtSecurityToken Verify(string jwt);
}