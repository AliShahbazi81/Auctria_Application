using Auctria_Application.Attribute;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Auctria_Application.Controllers.v1.AdminControllers;

public class ShoppingCartController : BaseAdminController
{
    private readonly IShoppingCartManager _shoppingCartManager;
    private readonly ILogger<ShoppingCartController> _logger;

    public ShoppingCartController(
        IShoppingCartManager shoppingCartManager, 
        ILogger<ShoppingCartController> logger)
    {
        _shoppingCartManager = shoppingCartManager;
        _logger = logger;
    }
    
    [HttpGet("GetUserShoppingCart")]
    [RequiredPermission(PermissionAction.ShoppingCart_List)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserShoppingCart(
        Guid userId, 
        Guid cartId, 
        CancellationToken cancellationToken,
        CurrencyTypes currencyType = CurrencyTypes.CAD)
    {
        try
        {
            return HandleResult(await _shoppingCartManager.GetUserCartAsync(cartId, cancellationToken, userId, currencyType));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to retrieve the shopping cart. Error: {e.Message}");
            throw;
        }
    }
    
    [HttpGet("GetUserShoppingCarts")]
    [RequiredPermission(PermissionAction.ShoppingCart_List)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserShoppingCarts(
        Guid userId, 
        CancellationToken cancellationToken,
        CurrencyTypes currencyType = CurrencyTypes.CAD)
    {
        try
        {
            return HandleResult(await _shoppingCartManager.GetUserCartsAsync(cancellationToken, userId, currencyType));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to retrieve the shopping carts. Error: {e.Message}");
            throw;
        }
    }
}