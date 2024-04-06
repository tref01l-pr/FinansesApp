namespace FinancesWebApi.Interfaces.Services;

public interface IPasswordSecurityService
{
    void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    public string EncodeEmail(string email);
}