using FinancesWebApi.Models;
using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Interfaces;

public interface IPhoneNumberRepository
{
    public bool IsCountryCodeExists(string countryCode);
    public bool IsNumberExists(string countryCode, string number);
    public CountryPhoneNumber GetCountryPhoneNumber(string countryCode);
    public UserPhoneNumber GetPhoneNumberWithUserId(int userId);
    public ICollection<CountryPhoneNumber> GetCountryPhoneNumbers();
    public bool UpdateUserNumber(UserPhoneNumber userPhoneNumber);
    public bool CreateUserNumber(UserPhoneNumber userPhoneNumber);
    public bool Save();
}