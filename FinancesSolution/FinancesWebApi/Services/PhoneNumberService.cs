using System.Text;
using FinancesWebApi.Interfaces.Services;

namespace FinancesWebApi.Services;

public class PhoneNumberService : IPhoneNumberService
{
    public string? GenerateCode()
    {
        Random random = new Random();
        StringBuilder codeBuilder = new StringBuilder();
        
        for (int i = 0; i < 12; i++)
            codeBuilder.Append(random.Next(0, 10));
        
        return codeBuilder.ToString();
    }
}