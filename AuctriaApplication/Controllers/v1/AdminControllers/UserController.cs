using Auctria_Application.Attribute;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Infrastructure.Membership.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Auctria_Application.Controllers.v1.AdminControllers;

public class UserController : BaseAdminController
{
    private readonly IUserManager _userManager;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserManager userManager, ILogger<UserController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet("Users")]
    [RequiredPermission(PermissionAction.Members_List)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            return HandleResult(await _userManager.GetUsersListAsync());
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while retrieving users list. Error {e.Message}");
            throw;
        }
    }
}