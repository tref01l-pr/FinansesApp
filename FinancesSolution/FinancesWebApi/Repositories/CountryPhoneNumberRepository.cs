using FinancesWebApi.Data;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Repositories;

public class CountryPhoneNumberRepository(DataContext context) : ICountryPhoneNumberRepository
{
    public bool IsCountryCodeExists(string countryCode) => 
        context.CountryPhoneNumbers.Any(n => n.Code == countryCode);

    public CountryPhoneNumber? GetCountryPhoneNumber(string countryCode) => 
        context.CountryPhoneNumbers.FirstOrDefault(n => n.Code == countryCode);

    public ICollection<CountryPhoneNumber>? GetCountryPhoneNumbers() => 
        context.CountryPhoneNumbers.OrderBy(n => n.Name).ToList();
}