using AuctriaApplication.DataAccess.Entities;
using AuctriaApplication.DataAccess.Entities.Users;

namespace AuctriaApplication.Services.Membership.Services.Token;

public interface ITokenService
{
    Task<string> GenerateAsync(User user);
}