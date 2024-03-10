using FinancesWebApi.Models.User;

namespace FinancesWebApi.Interfaces;

public interface IUserDeviceRepository
{
    Device GetDeviceByRefreshToken(string refreshToken);
    Device GetDeviceByUserId(int userId);
    bool UpdateRefreshToken(Device device, RefreshToken newRefreshToken);
    bool Save();

}