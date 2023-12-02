namespace AuctriaApplication.Services.MessagingAPI.Services.Sms;

public interface ISmsService
{
    Task<bool> SendAsync(
        string userPhone, 
        string body);
}