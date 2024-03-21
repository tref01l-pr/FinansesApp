using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Interfaces;

public interface ICountryPhoneNumberRepository
{
    public bool IsCountryCodeExists(string countryCode);
    public CountryPhoneNumber? GetCountryPhoneNumber(string countryCode);
    public ICollection<CountryPhoneNumber> GetCountryPhoneNumbers();
}