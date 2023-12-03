namespace AuctriaApplication.Services.MessagingAPI.Services.Email;

/// <summary>
/// Provides services for sending emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Asynchronously sends an email to a specified user.
    /// </summary>
    /// <param name="userEmail">The email address of the recipient.</param>
    /// <param name="emailTemplate">The content of the email to be sent.</param>
    /// <param name="emailSubject">The subject of the email.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the email was successfully sent.</returns>

    Task<bool> SendEmailAsync(
        string userEmail,
        string emailTemplate,
        string emailSubject);
}