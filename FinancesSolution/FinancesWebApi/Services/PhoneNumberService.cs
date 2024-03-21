using System.Text;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Models;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace FinancesWebApi.Services;

public class PhoneNumberService : IPhoneNumberService
{
    private int _lengthOfCode = 9;
    private readonly ITwilioRestClient _client;

    public PhoneNumberService(ITwilioRestClient twilioRestClient)
    {
        _client = twilioRestClient;
    }
    
    public string? GenerateCode()
    {
        Random random = new Random();
        StringBuilder codeBuilder = new StringBuilder();
        
        for (int i = 0; i < _lengthOfCode; i++)
            codeBuilder.Append(random.Next(0, 10));
        
        return codeBuilder.ToString();
    }

    public async Task<bool> SendSms(SmsModel model)
    {
        try
        {
            var message = await MessageResource.CreateAsync(
                body: model.Message,
                from: new PhoneNumber(model.From),
                to: new PhoneNumber(model.To),
                client: _client);

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    
    
}