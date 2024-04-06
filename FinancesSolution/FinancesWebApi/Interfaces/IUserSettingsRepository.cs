using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Interfaces;

public interface IUserSettingsRepository
{
    public Task<UserSettings?> GetUserSettingsByUserIdAsync(int userId);
}