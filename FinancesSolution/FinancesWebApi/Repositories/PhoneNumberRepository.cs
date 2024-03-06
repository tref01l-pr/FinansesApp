using FinancesWebApi.Data;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;

namespace FinancesWebApi.Repositories;

public class PhoneNumberRepository(DataContext context) : IPhoneNumberRepository
{
    public bool IsCountryCodeExists(string countryCode) => context.CountryPhoneNumbers.Any(n => n.Code == countryCode);
    public bool IsNumberExists(string countryCode, string number) => context.UserPhoneNumbers.Any(n => n.CountryCode == countryCode && n.Number == number);

    public CountryPhoneNumber GetCountryPhoneNumber(string countryCode) => context.CountryPhoneNumbers.FirstOrDefault(n => n.Code == countryCode)!;

    public ICollection<CountryPhoneNumber> GetCountryPhoneNumbers() => context.CountryPhoneNumbers.OrderBy(n => n.Name).ToList();
    public bool UpdateUserNumber(UserPhoneNumber userPhoneNumber)
    {
        context.Update(userPhoneNumber);

        return Save();
    }

    public bool CreateUserNumber(UserPhoneNumber userPhoneNumber)
    {
        context.Add(userPhoneNumber);

        return Save();
    }

    public bool Save() => context.SaveChanges() >= 0;
}