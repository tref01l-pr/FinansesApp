using FinancesWebApi.Dto;
using FinancesWebApi.Models;

namespace FinancesWebApi.Interfaces;

public interface IUserRepository
{
    ICollection<User> GetUsers();
    User? GetUser(int userId);
    User? GetUserByName(string userName);
    User? GetUserByEmail(string email);
    User? GetUserByNumber(NumberDto numberDto);
    bool IsUserExists(int userId);
    bool IsUserNameExists(string userName);
    bool IsEmailExists(string email);
    bool IsNumberExists(NumberDto numberDto);
    bool CreateUser(User user);
    bool UpdateUserEmail(User user);
    bool UpdateUserPhone(User user);
    bool DeleteUser(User user);
    bool Save();
}