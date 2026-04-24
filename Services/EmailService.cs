using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ASP.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mail = _config["MailSettings:Mail"];
        var pw = _config["MailSettings:Password"];
        var host = _config["MailSettings:Host"];
        var port = int.Parse(_config["MailSettings:Port"] ?? "587");

        var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(mail, pw),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(mail, _config["MailSettings:DisplayName"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}
