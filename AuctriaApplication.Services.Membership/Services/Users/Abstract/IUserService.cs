using AuctriaApplication.Services.Membership.Dto;
using AuctriaApplication.Services.Membership.Dto.ViewModel;

namespace AuctriaApplication.Services.Membership.Services.Users.Abstract;

public interface IUserService
{
    Task<List<UserViewModel>> GetUsersListAsync();
    
    Task<UserDto> CurrentUserAsync(Guid userId);

    Task<bool> SetTempCodeAsync(
        Guid userId,
        int validationType,
        string code,
        CancellationToken cancellationToken = default);
    
    Task<(bool, string?)> VerifyEmailAsync(
        Guid userId,
        int verificationType,
        string userCode);
    
    Task<(bool, string?)> VerifyPhoneAsync(
        Guid userId, 
        int verificationType,
        string userCode);
    
    Task<bool> LockOutAsync(
        Guid targetUserId, 
        int lockOutTime);

    Task<bool> UnLockAsync(Guid targetUserId);

    Task<bool> IsUserLockedAsync(
        Guid? userId = null,
        string? email = null);

}