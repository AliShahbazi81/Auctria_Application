namespace AuctriaApplication.Services.MessagingAPI.Services.Email;

public interface IEmailService
{
    Task<bool> SendEmailAsync(
        string userEmail,
        string emailTemplate,
        string emailSubject);
}