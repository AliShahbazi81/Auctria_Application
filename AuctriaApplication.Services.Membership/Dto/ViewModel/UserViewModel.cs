namespace AuctriaApplication.Services.Membership.Dto.ViewModel;

public record struct UserViewModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string? Role { get; set; }
    public string? TelegramId { get; set; }
    public string? WhatsappId { get; set; }
}