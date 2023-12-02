using AuctriaApplication.Services.MessagingAPI.Services.Sms.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AuctriaApplication.Services.MessagingAPI.Services.Sms;

public class SmsService : ISmsService
{
    private readonly SmsConfiguration _config;

    public SmsService(SmsConfiguration config)
    {
        _config = config;
        TwilioClient.Init(_config.AccountSid, _config.AuthToken);
    }

    public async Task<bool> SendAsync(
        string userPhone, 
        string body)
    {
        var messageOptions = CreateMessageOptions(userPhone, body);
    
        // Try to send the message and capture the result
        var messageResource = await MessageResource.CreateAsync(messageOptions);

        // Check if the message was sent successfully
        if (messageResource.Status == MessageResource.StatusEnum.Sent ||
            messageResource.Status == MessageResource.StatusEnum.Queued ||
            messageResource.Status == MessageResource.StatusEnum.Sending ||
            messageResource.Status == MessageResource.StatusEnum.Accepted)
            return true;
        return false;
    }

    private CreateMessageOptions CreateMessageOptions(string userPhone, string body)
    {
        return new CreateMessageOptions(new PhoneNumber(userPhone))
        {
            MessagingServiceSid = _config.MessagingServiceSid,
            From = new PhoneNumber(_config.From),
            Body = body
        };
    }
}