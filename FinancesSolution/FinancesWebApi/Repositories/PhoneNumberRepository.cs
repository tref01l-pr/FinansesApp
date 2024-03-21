using FinancesWebApi.Data;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Repositories;

public class PhoneNumberRepository(DataContext context) : IPhoneNumberRepository
{
    
    public bool IsNumberExists(string countryCode, string number) => 
        context.UserPhoneNumbers.Any(n => n.CountryCode == countryCode && n.Number == number);

    public UserPhoneNumber? GetPhoneNumberWithUserId(int userId) => 
        context.UserPhoneNumbers.FirstOrDefault(n => n.UserId == userId);

    
    public bool UpdateUserNumber(UserPhoneNumber userPhoneNumber)
    {
        context.Update(userPhoneNumber);         //should be fixed
        return Save();
    }

    public bool CreateUserNumber(UserPhoneNumber userPhoneNumber)
    {
        context.Add(userPhoneNumber);            //should be fixed
        return Save();
    }

    public bool Save() => context.SaveChanges() > 0;
}