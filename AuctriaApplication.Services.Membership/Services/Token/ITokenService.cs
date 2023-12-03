using AuctriaApplication.DataAccess.Entities;
using AuctriaApplication.DataAccess.Entities.Users;

namespace AuctriaApplication.Services.Membership.Services.Token;

/// <summary>
/// Provides services for generating authentication tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Asynchronously generates an authentication token for a given user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated token as a string.</returns>
    Task<string> GenerateAsync(User user);
}