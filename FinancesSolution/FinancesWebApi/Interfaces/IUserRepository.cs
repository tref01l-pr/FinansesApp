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
    bool IsUserWithIdExists(int userId);
    bool IsUserWithUserNameExists(string userName);
    bool IsUserWithEmailExists(string email);
    bool IsUserWithNumberExists(NumberDto numberDto);
    bool CreateUser(User user);
    bool UpdateUserEmail(User user);
    bool UpdateUserPhone(User user);
    bool DeleteUser(User user);
    bool Save();
}