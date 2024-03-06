using FinancesWebApi.Data;
using FinancesWebApi.Dto;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancesWebApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }


    public ICollection<User> GetUsers() => _context.Users.OrderBy(u => u.Id).ToList();

    public User? GetUser(int userId) => _context.Users.FirstOrDefault(u => u.Id == userId);

    public User? GetUserByName(string userName) => _context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefault(u => u.UserName == userName.ToLower());

    public User? GetUserByEmail(string email) => _context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefault(u => u.Email == email.ToLower());

    public User? GetUserByNumber(NumberDto numberDto) => _context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefault(u =>
        u.UserPhoneNumber.CountryCode == numberDto.CountryCode && u.UserPhoneNumber.Number == numberDto.Number.ToString());

    public bool IsUserWithIdExists(int userId) => _context.Users.Any(u => u.Id == userId);
    public bool IsUserWithUserNameExists(string userName) => _context.Users.Any(u => u.UserName == userName.ToLower());
    public bool IsUserWithEmailExists(string email) => _context.Users.Any(u => u.Email == email.ToLower());

    public bool IsUserWithNumberExists(NumberDto numberDto) => _context.Users.Any(u =>
        u.UserPhoneNumber.CountryCode == numberDto.CountryCode && u.UserPhoneNumber.Number == numberDto.Number.ToString());

    public bool CreateUser(User user)
    {
        user.Email = user.Email.ToLower();
        user.UserName = user.UserName.ToLower();
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        var roleEntity = _context.Roles.FirstOrDefault(r => r.Name == "user");

        if (roleEntity == null)
            return false;
        
        var userRole = new UserRole
        {
            Role = roleEntity,
            User = user
        };

        _context.Add(userRole);
        
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

        _context.Add(user);
        
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
        _context.Remove(user);
        return Save();
    }

    public bool Save() => _context.SaveChanges() >= 0;
}