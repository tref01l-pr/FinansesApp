using System.Net;
using System.Net.Mail;
using FinancesWebApi.Interfaces.Services;

namespace FinancesWebApi.Services;

public class EmailSender (IConfiguration configuration) : IEmailSender
{
    //smtp 
    private string _mail = configuration.GetSection("Smtp:User").Value!;
    private string _password = configuration.GetSection("Smtp:Password").Value!;
    private int _port = Convert.ToInt32(configuration.GetSection("Smtp:Port").Value!);
    
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