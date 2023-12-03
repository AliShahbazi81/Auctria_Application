using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Services.Membership.Dto;
using AuctriaApplication.Services.Membership.Dto.ViewModel;
using UserViewModel = AuctriaApplication.Services.Membership.Dto.ViewModel.UserViewModel;

namespace AuctriaApplication.Infrastructure.Membership.Services.Abstract;

/// <summary>
/// Manages user-related operations such as retrieval, registration, login, and verification.
/// </summary>
public interface IUserManager
{
    /// <summary>
    /// Asynchronously retrieves a list of all users.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of users.</returns>
    Task<Result<List<UsersViewModel>>> GetUsersListAsync();

    /// <summary>
    /// Asynchronously retrieves the current user's information.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the current user's information.</returns>
    Task<Result<UserViewModel>> GetCurrentUserAsync();

    /// <summary>
    /// Registers a new user asynchronously.
    /// </summary>
    /// <param name="userDto">The data transfer object containing the user's registration information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the registered user's view model.</returns>
    Task<Result<UserViewModel>> Register(RegisterDto userDto);

    /// <summary>
    /// Asynchronously logs in a user.
    /// </summary>
    /// <param name="loginDto">The data transfer object containing the user's login credentials.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the logged-in user's view model.</returns>
    Task<Result<UserViewModel>> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Sends an email verification asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string message indicating the outcome.</returns>
    Task<Result<string>> SendEmailVerificationAsync();

    /// <summary>
    /// Verifies a user's email asynchronously.
    /// </summary>
    /// <param name="code">The verification code sent to the user's email.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string message indicating the outcome.</returns>
    Task<Result<string>> VerifyEmailAsync(string code);

    /// <summary>
    /// Sends an SMS verification asynchronously.
    /// </summary>
    /// <param name="userPhone">The user's phone number to send the verification code to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string message indicating the outcome.</returns>
    Task<Result<string>> SendSmsVerificationAsync(string userPhone);

    /// <summary>
    /// Verifies a user's phone number asynchronously.
    /// </summary>
    /// <param name="userCode">The verification code sent to the user's phone.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string message indicating the outcome.</returns>
    Task<Result<string>> VerifyPhoneAsync(string userCode);

    /// <summary>
    /// Locks out a user asynchronously.
    /// </summary>
    /// <param name="targetUserId">The unique identifier of the user to lock out.</param>
    /// <param name="days">The number of days to lock out the user for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string message indicating the outcome.</returns>
    Task<Result<string>> LockoutUserAsync(
        Guid targetUserId,
        int days);

    /// <summary>
    /// Unlocks a user asynchronously.
    /// </summary>
    /// <param name="targetUserId">The unique identifier of the user to unlock.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string message indicating the outcome.</returns>
    Task<Result<string>> UnLockoutUserAsync(Guid targetUserId);
}