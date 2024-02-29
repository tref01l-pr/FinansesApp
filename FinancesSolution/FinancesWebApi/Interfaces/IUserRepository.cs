using FinancesWebApi.Models;

namespace FinancesWebApi.Interfaces;

public interface IUserRepository
{
    ICollection<User> GetUsers();
    User? GetUser(int userId);
    User? GetUserByName(string userName);
    User? GetUserByEmail(string email);
    bool UserExists(int userId);
    bool CreateUser(User user);
    bool UpdateUserEmail(User user);
    bool UpdateUserPhone(User user);
    bool DeleteUser(User user);
    bool Save();
}