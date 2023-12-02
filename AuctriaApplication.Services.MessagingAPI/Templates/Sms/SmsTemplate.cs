namespace AuctriaApplication.Services.MessagingAPI.Templates.Sms;

public static class SmsTemplate
{
    public static string Verification(string verificationCode)
    {
        return $"Your Verification code is : {verificationCode}";
    }
}