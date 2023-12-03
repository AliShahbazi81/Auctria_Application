using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Domain.Helper;
using AuctriaApplication.Domain.Variables;
using AuctriaApplication.Infrastructure.Membership.Guards;
using AuctriaApplication.Infrastructure.Membership.Services.Abstract;
using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Infrastructure.Services;
using AuctriaApplication.Services.Membership.Dto;
using AuctriaApplication.Services.Membership.Dto.ViewModel;
using AuctriaApplication.Services.Membership.Services.Users.Abstract;
using AuctriaApplication.Services.MessagingAPI.Services.Email;
using AuctriaApplication.Services.MessagingAPI.Services.Sms;
using AuctriaApplication.Services.MessagingAPI.Templates.Email;
using AuctriaApplication.Services.MessagingAPI.Templates.Sms;
using AuctriaApplication.Services.Validation.Services.Phone;

namespace AuctriaApplication.Infrastructure.Membership.Services;

public class UserManager : IUserManager
{
    private readonly IUserService _userService;
    private readonly IUserAccessor _userAccessor;
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;

    public UserManager(
        IUserService userService, 
        IUserAccessor userAccessor, 
        ISmsService smsService, 
        IEmailService emailService)
    {
        _userService = userService;
        _userAccessor = userAccessor;
        _smsService = smsService;
        _emailService = emailService;
    }
    
    public async Task<Result<List<UserViewModel>>> GetUsersListAsync()
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync())
            return Result<List<UserViewModel>>.Failure("User is locked");
        
        var users = await _userService.GetListAsync();

        return Result<List<UserViewModel>>.Success(users);
    }
    
    public async Task<Result<UserDto>> GetCurrentUserAsync()
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<UserDto>.Failure("User is locked");
        
        var user = await _userService.CurrentUserAsync(_userAccessor.GetUserId());

        return Result<UserDto>.Success(user);
    }

    public async Task<Result<UserDto>> RegisterOrLoginAsync(RegisterOrLoginDto userDto)
    {
        var loggedInUserDto = await _userService.RegisterOrLoginAsync(userDto);
        
        return loggedInUserDto is null 
            ? Result<UserDto>.Failure("Invalid credentials") 
            : Result<UserDto>.Success(loggedInUserDto);
    }
    
    public async Task<Result<string>> SendEmailVerificationAsync(
        Guid userId,
        string userEmail,
        string userFirstName,
        CancellationToken cancellationToken = default)
    {
        // Check if user has already verified their Email
        if (await _userService.IsFieldVerifiedAsync(userId, SharedUserFieldsVar.EmailConfirmed))
            return Result<string>.Failure("Your email has ben verified already!");

        // Generate 6 digits code
        var generatedCode = CodeGenerator.Generate();
        var emailTemplate = EmailTemplate.Verification(generatedCode, userFirstName);

        // Send email verification along with code to user
        if (!await _emailService.SendEmailAsync(userEmail, emailTemplate, "Verification Code"))
            return Result<string>.Failure("Failed to send verification code!");

        // Save the verification code for user
        return !await _userService.SetTempCodeAsync(userId, (int)ValidationTypes.Email, generatedCode, cancellationToken)
            ? Result<string>.Failure("Failed to save code!")
            : Result<string>.Success("Code has been sent successfully!");
    }

    public async Task<Result<string>> VerifyEmailAsync(string code)
    {
        if (UserManagerGuard.IsCodeValid(code))
            return Result<string>.Failure("The entered code is incorrect");

        var verification = await _userService.VerifyEmailAsync(_userAccessor.GetUserId(), (int)ValidationTypes.Email, code);

        return !verification.Item1 
            ? Result<string>.Failure(verification.Item2!) 
            : Result<string>.Success("Your email has been verified!");
    }

    public async Task<Result<string>> SendSmsVerificationAsync(
        Guid userId,
        string userPhone,
        CancellationToken cancellationToken = default)
    {
        // Check if phone number is valid
        if(!PhoneValidationService.IsPhoneNumberValid(userPhone))
            return Result<string>.Failure("Sorry, but it seems the phone number you entered is not valid");
        
        // Check if user has already verified their Phone Number already
        if (await _userService.IsFieldVerifiedAsync(userId, SharedUserFieldsVar.PhoneNumberConfirmed))
            return Result<string>.Failure("Your Phone number has ben verified already!");

        // Generate 6 digits code
        var generatedCode = CodeGenerator.Generate();
        var emailTemplate = SmsTemplate.Verification(generatedCode);

        // Send code verification 
        if (!await _smsService.SendAsync(userPhone, emailTemplate))
            return Result<string>.Failure("Failed to send verification code!");

        // Save the verification code for user
        return !await _userService.SetTempCodeAsync(
            userId, 
            (int)ValidationTypes.Sms, 
            generatedCode, 
            cancellationToken)
            
            ? Result<string>.Failure("Failed to save code!")
            : Result<string>.Success("Code has been sent successfully!");
    }

    public async Task<Result<string>> VerifyPhoneAsync(string userCode)
    {
        if (UserManagerGuard.IsCodeValid(userCode))
            return Result<string>.Failure("Please enter the verification code!");

        var verification = await _userService.VerifyPhoneAsync(
            _userAccessor.GetUserId(), 
            (int)ValidationTypes.Email, 
            userCode);

        return !verification.Item1 
            ? Result<string>.Failure(verification.Item2!) 
            : Result<string>.Success("Your phone has been verified!");
    }
}