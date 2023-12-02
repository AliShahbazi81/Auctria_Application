using PhoneNumbers;

namespace AuctriaApplication.Services.Validation.Services.Phone;

public static class PhoneValidationService
{
    public static bool IsPhoneNumberValid(string phoneNumber)
    {
        var util = PhoneNumberUtil.GetInstance();
        var region = GetPhoneRegion(phoneNumber);
        var numberProto = util.Parse(phoneNumber, region);

        return util.IsValidNumber(numberProto);
    }

    private static string GetPhoneRegion(string phoneNumber)
    {
        var util = PhoneNumberUtil.GetInstance();
        var numberProto = util.Parse(phoneNumber, null);
        return util.GetRegionCodeForNumber(numberProto);
    }
}