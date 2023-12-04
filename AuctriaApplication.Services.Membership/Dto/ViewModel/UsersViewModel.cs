namespace AuctriaApplication.Services.Membership.Dto.ViewModel;

public record struct UsersViewModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string? Role { get; set; }
}