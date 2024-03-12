using FinancesWebApi.Dto;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User;

namespace FinancesWebApi.Interfaces;

public interface IUserRepository
{
    ICollection<User> GetUsers();
    User? GetUser(int userId);
    User? GetUserByName(string userName);
    User? GetUserByEmail(string email);
    User? GetUserByNumber(NumberDto numberDto);
    User? GetUserByVerificationEmailToken(string token);
    User? GetUserByPasswordResetToken(string token);
    string GetRandomPasswordResetToken();
    string GetRandomVerificationEmailToken();
    bool IsUserWithIdExists(int userId);
    bool IsUserWithUserNameExists(string userName);
    bool IsUserWithEmailExists(string email);
    bool IsUserWithPasswordResetTokenExist(string resetToken);
    bool IsUserWithVerificationEmailTokenExist(string emailToken);
    bool IsUserWithNumberExists(NumberDto numberDto);
    bool CreateUser(User user);
    bool UpdateUser(User user);
    bool DeleteUser(User user);
    bool Save();
}