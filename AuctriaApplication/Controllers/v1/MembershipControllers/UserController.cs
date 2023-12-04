using AuctriaApplication.Infrastructure.Membership.Services.Abstract;
using AuctriaApplication.Services.Membership.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctria_Application.Controllers.v1.MembershipControllers;

public class UserController : BaseApiController
{
    private readonly IUserManager _userManager;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserManager userManager, 
        ILogger<UserController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet("CurrentUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            return HandleResult(await _userManager.GetCurrentUserAsync());
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while retrieving the current user. Error: {e.Message}");
            throw;
        }
    }
    
    [HttpPost("Login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        try
        {
            return HandleResult(await _userManager.LoginAsync(loginDto));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to login the user: {e.Message}");
            throw;
        }
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        try
        {
            return HandleResult(await _userManager.Register(registerDto));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to register the new user: {e.Message}");
            throw;
        }
    }

    [HttpPost("SendEmailVerificationCode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendEmailVerificationCode()
    {
        try
        {
            return HandleResult(await _userManager.SendEmailVerificationAsync());
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to sending email verification code to user: {e.Message}");
            throw;
        }
    }

    [HttpPost("SendSmsVerificationCode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendSmsVerificationCode(string userPhone)
    {
        try
        {
            return HandleResult(await _userManager.SendSmsVerificationAsync(userPhone));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to sending sms verification code to user: {e.Message}");
            throw;
        }
    }

    [HttpPut("VerifyEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyEmail(string code)
    {
        try
        {
            return HandleResult(await _userManager.VerifyEmailAsync(code));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to verify user's email address: {e.Message}");
            throw;
        }
    }

    [HttpPut("VerifyPhone")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyPhone(string code)
    {
        try
        {
            return HandleResult(await _userManager.VerifyPhoneAsync(code));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to verify user's phone number: {e.Message}");
            throw;
        }
    }
}