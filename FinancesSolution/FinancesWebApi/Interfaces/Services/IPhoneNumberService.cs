using FinancesWebApi.Models;

namespace FinancesWebApi.Interfaces.Services;

public interface IPhoneNumberService
{
    string? GenerateCode();
    Task<bool> SendSms(SmsModel model);
}