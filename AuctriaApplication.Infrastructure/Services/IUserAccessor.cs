namespace AuctriaApplication.Infrastructure.Services;

public interface IUserAccessor
{
    Guid GetUserId();
    string? GetUserEmail();
}