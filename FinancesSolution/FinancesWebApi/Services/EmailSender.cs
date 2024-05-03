using System.Net;
using System.Net.Mail;
using DotNetEnv;
using FinancesWebApi.Interfaces.Services;

namespace FinancesWebApi.Services;

public class EmailSender : IEmailSender
{
    //smtp 
    private readonly string _mail;
    private readonly string _password;
    private readonly int _port;

    public EmailSender()
    {
        Env.Load();

        _mail = Environment.GetEnvironmentVariable("SMTP_USER") ?? throw new Exception("SMTP_USER is not set in .env file.");
        _password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new Exception("SMTP_PASSWORD is not set in .env file.");
        _port = Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_PORT") ?? throw new Exception("SMTP_PORT is not set in .env file."));
    }
    
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp.gmail.com", _port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_mail, _password)
        };

        return client.SendMailAsync(
            new MailMessage(from: _mail,
                to: email,
                subject,
                message
            ));
    }
}