using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace FinancesWebApi.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private string _secureKey = "this is a very secure key1234567";

    public string Generate(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        Console.WriteLine("user.UserRoles = " + user.UserRoles.Count);
        
        foreach (var role in user.UserRoles)
            claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration.GetSection("Jwt:Token").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public JwtSecurityToken Verify(string jwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secureKey);
        tokenHandler.ValidateToken(jwt, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false
        }, out SecurityToken validatedToken);
        return (JwtSecurityToken) validatedToken;
    }
}