namespace AuctriaApplication.Services.MessagingAPI.Services.Sms;

/// <summary>
/// Provides services for sending SMS messages.
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Asynchronously sends an SMS message to a specified phone number.
    /// </summary>
    /// <param name="userPhone">The phone number of the recipient.</param>
    /// <param name="body">The content of the SMS message to be sent.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the SMS message was successfully sent.</returns>
    Task<bool> SendAsync(
        string userPhone, 
        string body);
}