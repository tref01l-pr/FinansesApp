using FinancesWebApi.Models.User;

namespace FinancesWebApi.Interfaces;

public interface IUserDeviceRepository
{
    Device? GetDeviceById(int deviceId);
    Device? GetDeviceByRefreshToken(string refreshToken);
    List<Device> GetDevicesByUserId(int userId);
    bool UpdateRefreshToken(Device device, RefreshToken newRefreshToken);
    bool CreateDevice(Device device);
    bool DeleteDevice(Device device);
    bool Save();

}