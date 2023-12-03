using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Services.Membership.Dto;
using AuctriaApplication.Services.Membership.Dto.ViewModel;

namespace AuctriaApplication.Infrastructure.Membership.Services.Abstract;

public interface IUserManager
{
    Task<Result<List<UserViewModel>>> GetUsersListAsync();

    Task<Result<UserDto>> GetCurrentUserAsync();

    Task<Result<UserDto>> RegisterOrLoginAsync(RegisterOrLoginDto userDto);

    Task<Result<string>> SendEmailVerificationAsync(
        Guid userId,
        string userEmail,
        string userFirstName,
        CancellationToken cancellationToken = default);

    Task<Result<string>> VerifyEmailAsync(string code);

    Task<Result<string>> SendSmsVerificationAsync(
        Guid userId,
        string userPhone,
        CancellationToken cancellationToken = default);

    Task<Result<string>> VerifyPhoneAsync(string userCode);
}