using FinancesWebApi.Data;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models;

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

    public User? GetUserByName(string userName) => _context.Users.FirstOrDefault(u => u.UserName == userName.ToLower());

    public User? GetUserByEmail(string email) => _context.Users.FirstOrDefault(u => u.Email == email.ToLower());

    public bool UserExists(int userId) => _context.Users.Any(u => u.Id == userId);
    
    

    public bool CreateUser(User user)
    {
        user.Email = user.Email.ToLower();
        user.UserName = user.UserName.ToLower();

        _context.Add(user);
        
        UserSettings userSettings = new UserSettings()
        {
            UserId = user.Id,
        };
        
        Account defaultAccount = new Account()
        {
            UserId = user.Id,
            Title = "Main"
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
        return false;
    }

    public bool Save() => _context.SaveChanges() >= 0;
}