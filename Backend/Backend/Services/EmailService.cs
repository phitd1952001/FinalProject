using Backend.Services.IServices;
using MailKit.Security;
using MimeKit;

namespace Backend.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
        _logger.LogInformation("Create MailService");
    }
    public async Task Send(string to, string subject, string html)
    {
        var email = new MimeMessage();
        email.Sender = new MailboxAddress(AppSettings.DisplayName, AppSettings.Mail);
        email.From.Add(new MailboxAddress(AppSettings.DisplayName, AppSettings.Mail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;


        var builder = new BodyBuilder();
        builder.HtmlBody = html;
        email.Body = builder.ToMessageBody();

        // dùng SmtpClient của MailKit
        using var smtp = new MailKit.Net.Smtp.SmtpClient(); //using gửi xong xóa để k làm chậm hệ thống

        try
        {
            smtp.Connect(AppSettings.Host, AppSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(AppSettings.Mail, AppSettings.Password);
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
            System.IO.Directory.CreateDirectory("mailssave");
            var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
            await email.WriteToAsync(emailsavefile);

            _logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
            _logger.LogError(ex.Message);
        }

        smtp.Disconnect(true);

        _logger.LogInformation("send mail to " + to);
    }
}