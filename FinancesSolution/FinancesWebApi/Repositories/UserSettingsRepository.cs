using FinancesWebApi.Data;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Repositories;

public class UserSettingsRepository(DataContext context) : IUserSettingsRepository
{
    public async Task<UserSettings?> GetUserSettingsByUserIdAsync(int userId) =>
        context.UserSettings.FirstOrDefault(us => us.UserId == userId);
}