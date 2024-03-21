using Twilio.Clients;
using Twilio.Http;
using HttpClient = Twilio.Http.HttpClient;

namespace FinancesWebApi.Services;

public class TwilioClient : ITwilioRestClient
{
    private readonly ITwilioRestClient _innerClient;
    public TwilioClient(IConfiguration configuration, System.Net.Http.HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "CustomTwilioRestClient-Demo");
        _innerClient = new TwilioRestClient(
            configuration.GetSection("Twilio:AccountSid").Value,
            configuration.GetSection("Twilio:AuthToken").Value,
            httpClient: new SystemNetHttpClient(httpClient));
    }

    public Response Request(Request request) => _innerClient.Request(request);
    public Task<Response> RequestAsync(Request request) => _innerClient.RequestAsync(request);
    public string AccountSid => _innerClient.AccountSid;
    public string Region => _innerClient.Region;
    public HttpClient HttpClient => _innerClient.HttpClient;
}