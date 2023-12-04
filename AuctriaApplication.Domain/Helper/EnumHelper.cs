namespace AuctriaApplication.Domain.Helper;

public static class EnumHelper
{
    public static string ConvertEnumValueToMatchingString<TEnum>(int value) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
            throw new InvalidOperationException("The provided value is not a valid enum value.");

        var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);
        return enumValue.ToString();
    }


    public static T ConvertToEnum<T>(string value) where T : struct
    {
        // Normalize the input value: lowercase and remove spaces
        var normalizedInput = value.ToLower().Replace(" ", "");

        foreach (var enumName in Enum.GetNames(typeof(T)))
        {
            // Normalize the enum names in the same way
            var normalizedEnumName = enumName.ToLower().Replace(" ", "");
            if (normalizedEnumName != normalizedInput) 
                continue;
            // Parse and return the enum value if matched
            if (Enum.TryParse<T>(enumName, true, out var result))
                return result;
        }
        
        throw new InvalidOperationException("The provided string does not match any Enum value.");
    }
}