namespace AuctriaApplication.Services.Payment.Services.Configuration;

public record PaymentConfig
{
    public required string TokenKey { get; set; }
    public required string SecretKey { get; set; }
}