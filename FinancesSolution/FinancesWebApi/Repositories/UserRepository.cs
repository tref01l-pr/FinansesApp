using FinancesWebApi.Data;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User;
using FinancesWebApi.Models.User.UserSettings;
using Microsoft.EntityFrameworkCore;

namespace FinancesWebApi.Repositories;

public class UserRepository(DataContext context) : IUserRepository
{
    public ICollection<User> GetUsers() => context.Users.OrderBy(u => u.Id).ToList();

    public User? GetUser(int userId) => context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefault(u => u.Id == userId);

    public User? GetUserByName(string userName) => context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefault(u => u.UserName == userName.ToLower());

    public User? GetUserByEmail(string email) => context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefault(u => u.Email == email.ToLower());

    public User? GetUserByNumber(NumberDto numberDto) => context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefault(u =>
        u.UserPhoneNumber.CountryCode == numberDto.CountryCode && u.UserPhoneNumber.Number == numberDto.Number.ToString());

    public bool IsUserWithIdExists(int userId) => context.Users.Any(u => u.Id == userId);
    public bool IsUserWithUserNameExists(string userName) => context.Users.Any(u => u.UserName == userName.ToLower());
    public bool IsUserWithEmailExists(string email) => context.Users.Any(u => u.Email == email.ToLower());

    public bool IsUserWithNumberExists(NumberDto numberDto) => context.Users.Any(u =>
        u.UserPhoneNumber.CountryCode == numberDto.CountryCode && u.UserPhoneNumber.Number == numberDto.Number.ToString());

    public bool CreateUser(User user)
    {
        user.Email = user.Email.ToLower();
        user.UserName = user.UserName.ToLower();
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        var roleEntity = context.Roles.FirstOrDefault(r => r.Name == "user");

        if (roleEntity == null)
            return false;
        
        var userRole = new UserRole
        {
            Role = roleEntity,
            User = user
        };

        context.Add(userRole);
        
        UserSettings userSettings = new UserSettings()
        {
            NickName = user.UserName,
            DateOfRegistration = DateTime.Now,
            User = user
        };
        
        Account defaultAccount = new Account()
        {
            User = user,
            Title = "main",
            Comments = "Main Account"
        };
        
        user.UserSettings = userSettings;
        user.Accounts.Add(defaultAccount);

        context.Add(user);
        
        return Save();
    }

    public bool UpdateUserEmail(User user)
    {
        return false;
    }

    public bool UpdateUserPhone(User user)
    {
        return false;
    }

    public bool DeleteUser(User user)
    {
        context.Remove(user);
        return Save();
    }

    public bool Save() => context.SaveChanges() >= 0;
    
}