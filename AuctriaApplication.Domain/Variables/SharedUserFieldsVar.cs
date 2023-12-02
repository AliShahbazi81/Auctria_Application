namespace AuctriaApplication.Domain.Variables;

public record struct SharedUserFieldsVar
{
    public const string EmailConfirmed = "EmailConfirmed";
    public const string PhoneNumberConfirmed = "PhoneNumberConfirmed";
    public const string TwoFactorEnabled = "TwoFactorEnabled";
    public const string LockoutEnabled = "LockoutEnabled";
}