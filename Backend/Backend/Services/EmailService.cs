using Backend.Services.IServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Backend.Services;

public class EmailService : IEmailService
{
    public void Send(string to, string subject, string html, string from = null)
    {
        // create message
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ?? AppSettings.EmailFrom));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        // send email
        using var smtp = new SmtpClient();
        smtp.Connect(AppSettings.SmtpHost, AppSettings.SmtpPort, SecureSocketOptions.StartTls);
        smtp.Authenticate(AppSettings.SmtpUser, AppSettings.SmtpPass);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}