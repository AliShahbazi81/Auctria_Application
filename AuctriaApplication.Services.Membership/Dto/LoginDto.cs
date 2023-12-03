namespace AuctriaApplication.Services.Membership.Dto;

public record struct LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}