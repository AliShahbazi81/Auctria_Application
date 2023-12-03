using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Services.Membership.Dto;
using AuctriaApplication.Services.Membership.Dto.ViewModel;
using UserViewModel = AuctriaApplication.Services.Membership.Dto.UserViewModel;

namespace AuctriaApplication.Infrastructure.Membership.Services.Abstract;

public interface IUserManager
{
    Task<Result<List<UsersViewModel>>> GetUsersListAsync();

    Task<Result<UserViewModel>> GetCurrentUserAsync();

    Task<Result<UserViewModel>> Register(RegisterDto userDto);

    Task<Result<UserViewModel>> LoginAsync(LoginDto loginDto);

    Task<Result<string>> SendEmailVerificationAsync();

    Task<Result<string>> VerifyEmailAsync(string code);

    Task<Result<string>> SendSmsVerificationAsync(string userPhone);

    Task<Result<string>> VerifyPhoneAsync(string userCode);

    Task<Result<string>> LockoutUserAsync(
        Guid targetUserId,
        int days);

    Task<Result<string>> UnLockoutUserAsync(Guid targetUserId);
}