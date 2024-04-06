using System.Text;
using FinancesWebApi.Interfaces.Services;

namespace FinancesWebApi.Services;

public class PasswordSecurityService : IPasswordSecurityService
{
    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        passwordHash = Encoding.UTF8.GetBytes(hashedPassword);
        passwordSalt = Encoding.UTF8.GetBytes(salt);
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        string hashedPasswordFromBytes = Encoding.UTF8.GetString(passwordHash);
        string saltFromBytes = Encoding.UTF8.GetString(passwordSalt);

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, saltFromBytes);

        return hashedPassword.Equals(hashedPasswordFromBytes);
    }
    
    public string EncodeEmail(string email)
    {
        var emailParts = email.Split('@');
        StringBuilder resultBuilder = new StringBuilder();
    
        int lengthToTake = Math.Min(emailParts[0].Length, 1);
        resultBuilder.Append(emailParts[0], 0, lengthToTake);
        resultBuilder.Append("********");
        resultBuilder.Append('@').Append(emailParts[1]);

        return resultBuilder.ToString();
    }
}