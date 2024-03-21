using dotenv.net;
using Twilio.Clients;
using Twilio.Http;
using HttpClient = Twilio.Http.HttpClient;

namespace FinancesWebApi.Services;

public class TwilioClient : ITwilioRestClient
{
    private readonly ITwilioRestClient _innerClient;
    public TwilioClient(System.Net.Http.HttpClient httpClient)
    {
        DotNetEnv.Env.Load();
        
        httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "CustomTwilioRestClient-Demo");
        _innerClient = new TwilioRestClient(
            Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"),
            Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"),
            httpClient: new SystemNetHttpClient(httpClient));
    }

    public Response Request(Request request) => _innerClient.Request(request);
    public Task<Response> RequestAsync(Request request) => _innerClient.RequestAsync(request);
    public string AccountSid => _innerClient.AccountSid;
    public string Region => _innerClient.Region;
    public HttpClient HttpClient => _innerClient.HttpClient;
}