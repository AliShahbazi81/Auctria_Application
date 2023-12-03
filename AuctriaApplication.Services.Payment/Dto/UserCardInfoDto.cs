namespace AuctriaApplication.Services.Payment.Dto;

public record struct UserCardInfoDto
{
    public required string CardNumber { get; set; }
    public required string HolderName { get; set; }
    public required string Cvv { get; set; }
    public required string Year { get; set; }
    public required string Month { get; set; }
}