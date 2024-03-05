using System.IdentityModel.Tokens.Jwt;
using FinancesWebApi.Models;

namespace FinancesWebApi.Interfaces.Services;

public interface IJwtService
{
    string Generate(User user);
    JwtSecurityToken Verify(string jwt);
}