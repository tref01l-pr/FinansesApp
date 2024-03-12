using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Interfaces;

public interface IUserRoleRepository
{
    Role? GetRoleByName(string name);
}