using FinancesWebApi.Data;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models.User;

namespace FinancesWebApi.Repositories;

public class UserDeviceRepository(DataContext context) : IUserDeviceRepository
{
    public Device GetDeviceById(int deviceId) => context.Devices.FirstOrDefault(d => d.Id == deviceId)!;

    public Device GetDeviceByRefreshToken(string refreshToken) =>
        context.Devices.FirstOrDefault(d => d.Token == refreshToken)!;

    public List<Device> GetDevicesByUserId(int userId) => context.Devices.Where(d => d.UserId == userId).ToList();

    public bool UpdateRefreshToken(Device device, RefreshToken newRefreshToken)
    {
        device.Token = newRefreshToken.Token;
        device.TokenCreated = newRefreshToken.Created;
        device.TokenExpires = newRefreshToken.Expires;

        context.Update(device);
        
        return Save();
    }

    public bool CreateDevice(Device device)
    {
        context.Add(device);

        return Save();
    }

    public bool DeleteDevice(Device device)
    {
        context.Remove(device);

        return Save();
    }

    public bool Save() => context.SaveChanges() > 0;
}