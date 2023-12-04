namespace AuctriaApplication.Services.MessagingAPI.Services.Sms.Configuration;

public class SmsConfiguration
{
    public required string AccountSid { get; set; }
    public required string SecretKey { get; set; }
    public required string From { get; set; }
    public required string MessagingServiceSid { get; set; }
}