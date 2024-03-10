using FinancesWebApi.Data;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models.User;

namespace FinancesWebApi.Repositories;

public class UserDeviceRepository(DataContext context) : IUserDeviceRepository
{
    public Device GetDeviceByRefreshToken(string refreshToken) =>
        context.Devices.FirstOrDefault(d => d.Token == refreshToken)!;

    public Device GetDeviceByUserId(int userId)
    {
        throw new NotImplementedException();
    }

    public bool UpdateRefreshToken(Device device, RefreshToken newRefreshToken)
    {
        device.Token = newRefreshToken.Token;
        device.TokenCreated = newRefreshToken.Created;
        device.TokenExpires = newRefreshToken.Expires;

        context.Update(device);
        
        return Save();
    }
    
    public bool Save() => context.SaveChanges() >= 0;
}