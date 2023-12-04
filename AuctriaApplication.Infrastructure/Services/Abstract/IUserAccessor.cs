namespace AuctriaApplication.Infrastructure.Services.Abstract;

/// <summary>
/// Provides access to the current user's information.
/// </summary>
public interface IUserAccessor
{
    /// <summary>
    /// Retrieves the unique identifier of the current user.
    /// </summary>
    /// <returns>The unique identifier (GUID) of the current user.</returns>
    Guid GetUserId();
    
    /// <summary>
    /// Retrieves the email address of the current user.
    /// </summary>
    /// <returns>The email address of the current user.</returns>
    string GetUserEmail();
    
    /// <summary>
    /// Retrieves the username of the current user.
    /// </summary>
    /// <returns>The username of the current user.</returns>
    string GetUserUsername();

    string GetUserRole();
}