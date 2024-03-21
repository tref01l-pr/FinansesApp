using FinancesWebApi.Models;
using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Interfaces;

public interface IPhoneNumberRepository
{
    public bool IsNumberExists(string countryCode, string number);
    public UserPhoneNumber? GetPhoneNumberWithUserId(int userId);
    public bool UpdateUserNumber(UserPhoneNumber userPhoneNumber);
    public bool CreateUserNumber(UserPhoneNumber userPhoneNumber);
    public bool Save();
}