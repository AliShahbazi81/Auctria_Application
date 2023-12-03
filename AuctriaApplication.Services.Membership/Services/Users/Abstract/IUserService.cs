using AuctriaApplication.Services.Membership.Dto;
using AuctriaApplication.Services.Membership.Dto.ViewModel;
using UserViewModel = AuctriaApplication.Services.Membership.Dto.ViewModel.UserViewModel;

namespace AuctriaApplication.Services.Membership.Services.Users.Abstract;

/// <summary>
/// Provides user-related services including registration, login, verification, and user management.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Asynchronously retrieves a list of all users.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of user view models.</returns>
    Task<List<UsersViewModel>> GetListAsync();

    /// <summary>
    /// Registers a new user asynchronously.
    /// </summary>
    /// <param name="registerDto">The data transfer object containing the user's registration information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the registered user's view model.</returns>
    Task<UserViewModel> RegisterAsync(RegisterDto registerDto);

    /// <summary>
    /// Asynchronously logs in a user.
    /// </summary>
    /// <param name="loginDto">The data transfer object containing the user's login credentials.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the logged-in user's view model, or null if login fails.</returns>
    Task<UserViewModel?> LoginAsync(LoginDto loginDto);
    
    /// <summary>
    /// Retrieves the current user's information based on their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the current user's view model.</returns>
    Task<UserViewModel> CurrentUserAsync(Guid userId);

    /// <summary>
    /// Asynchronously sets a temporary code for a user, typically for verification purposes.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="validationType">The type of validation the code is for.</param>
    /// <param name="code">The temporary code to set.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the operation was successful.</returns>
    Task<bool> SetTempCodeAsync(
        Guid userId,
        int validationType,
        string code,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously verifies a user's email.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="verificationType">The type of verification.</param>
    /// <param name="userCode">The verification code provided by the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple indicating success and an optional error message.</returns>
    Task<(bool, string?)> VerifyEmailAsync(
        Guid userId,
        int verificationType,
        string userCode);
    
    /// <summary>
    /// Asynchronously verifies a user's phone number.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="verificationType">The type of verification.</param>
    /// <param name="userCode">The verification code provided by the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple indicating success and an optional error message.</returns>
    Task<(bool, string?)> VerifyPhoneAsync(
        Guid userId, 
        int verificationType,
        string userCode);
    
    /// <summary>
    /// Locks out a user for a specified number of days.
    /// </summary>
    /// <param name="targetUserId">The unique identifier of the user to lock out.</param>
    /// <param name="days">The number of days to lock out the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the operation was successful.</returns>
    Task<bool> LockOutAsync(
        Guid targetUserId, 
        int days);

    /// <summary>
    /// Unlocks a user.
    /// </summary>
    /// <param name="targetUserId">The unique identifier of the user to unlock.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the operation was successful.</returns>
    Task<bool> UnLockAsync(Guid targetUserId);

    /// <summary>
    /// Retrieves the email addresses of all super admin users.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of email addresses.</returns>
    Task<IEnumerable<string?>> GetSuperAdminEmailsAsync();
    
    /// <summary>
    /// Checks if a user is locked out.
    /// </summary>
    /// <param name="userId">The unique identifier of the user. Optional.</param>
    /// <param name="email">The email of the user. Optional.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the user is locked out.</returns>
    Task<bool> IsUserLockedAsync(
        Guid? userId = null,
        string? email = null);

    /// <summary>
    /// Checks if a specific field of a user is verified.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="fieldName">The name of the field to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the specified field is verified.</returns>
    Task<bool> IsFieldVerifiedAsync(
        Guid userId,
        string fieldName);

}