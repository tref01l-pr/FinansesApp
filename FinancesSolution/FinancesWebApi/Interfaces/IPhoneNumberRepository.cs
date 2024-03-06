using FinancesWebApi.Models;

namespace FinancesWebApi.Interfaces;

public interface IPhoneNumberRepository
{
    public bool IsCountryCodeExists(string countryCode);
    public bool IsNumberExists(string countryCode, string number);
    public CountryPhoneNumber GetCountryPhoneNumber(string countryCode);
    public ICollection<CountryPhoneNumber> GetCountryPhoneNumbers();
    public bool UpdateUserNumber(UserPhoneNumber userPhoneNumber);
    public bool CreateUserNumber(UserPhoneNumber userPhoneNumber);
    public bool Save();
}