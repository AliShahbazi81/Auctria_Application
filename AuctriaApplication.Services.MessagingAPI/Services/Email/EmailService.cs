using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace AuctriaApplication.Services.MessagingAPI.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendEmailAsync(
        string userEmail,
        string emailTemplate,
        string emailSubject,
        CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["EmailCredentials:Email"]));
        email.To.Add(MailboxAddress.Parse(userEmail));
        email.Subject = emailSubject;
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = emailTemplate,
        };

        // send email
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["EmailCredentials:Host"],
            587, 
            SecureSocketOptions.StartTls,
            cancellationToken);
        
        // Check if the task has been cancelled
        if (cancellationToken.IsCancellationRequested)
        {
            await smtp.DisconnectAsync(true, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        await smtp.AuthenticateAsync(
            _config["EmailCredentials:Email"],
            _config["EmailCredentials:Password"], 
            cancellationToken);

        await smtp.SendAsync(email, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);

        return true;
    }
}