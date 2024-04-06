using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User;
using Microsoft.IdentityModel.Tokens;

namespace FinancesWebApi.Services;

public class JwtService : IJwtService
{
    private string _jwt;
    public JwtService()
    {
        DotNetEnv.Env.Load();

        _jwt = Environment.GetEnvironmentVariable("JWT_TOKEN") ?? throw new Exception("JWT_TOKEN is not set in .env file.");
    }
    
    public AccessToken Generate(User? user, int deviceId)
    {
        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new ("deviceId", deviceId.ToString())
        };
        
        foreach (var role in user.UserRoles)
            claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _jwt));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var created = DateTime.Now;
        var expires = created.AddHours(2);
        
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessToken
        {
            Token = jwt,
            Created = created,
            Expires = expires
        };
    }

    public RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7)
        };

        return refreshToken;
    }

    public JwtSecurityToken Verify(string jwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwt);
        tokenHandler.ValidateToken(jwt, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false
        }, out SecurityToken validatedToken);
        return (JwtSecurityToken) validatedToken;
    }

    public string GenerateRandomToken() => Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
}