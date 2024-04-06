using FinancesWebApi.Models.User.UserSettings;
using Task = Twilio.TwiML.Voice.Task;

namespace FinancesWebApi.Interfaces;

public interface IUserRoleRepository
{
    Role? GetRoleByName(string name);
    Task<List<UserRole>> GetRolesByUserIdAsync(int id);
}