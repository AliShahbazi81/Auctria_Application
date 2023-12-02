namespace AuctriaApplication.Services.MessagingAPI.Services.Sms;

public interface ISmsService
{
    Task<(bool, string)> SendAsync(
        string userPhone,
        string body);
}