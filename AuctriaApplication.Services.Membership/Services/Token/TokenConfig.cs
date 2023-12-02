namespace AuctriaApplication.Services.Membership.Services.Token;

public record TokenConfig
{
    public string TokenKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}