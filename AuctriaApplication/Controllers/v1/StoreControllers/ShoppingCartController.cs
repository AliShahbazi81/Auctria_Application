using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Auctria_Application.Controllers.v1.StoreControllers;

public class ShoppingCartController : BaseApiController
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
    
    [HttpGet("GetShoppingCart")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetShoppingCart(
        Guid cartId, 
        CancellationToken cancellationToken, 
        CurrencyTypes currencyType = CurrencyTypes.CAD)
    {
        try
        {
            return HandleResult(await _shoppingCartManager.GetUserCartAsync(cartId, cancellationToken, currencyType: currencyType));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to retrieve the shopping cart. Error: {e.Message}");
            throw;
        }
    }
    
    [HttpGet("GetShoppingCarts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetShoppingCarts(
        CancellationToken cancellationToken, 
        CurrencyTypes currencyType = CurrencyTypes.CAD)
    {
        try
        {
            return HandleResult(await _shoppingCartManager.GetUserCartsAsync(cancellationToken, currencyType: currencyType));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to retrieve the shopping carts. Error: {e.Message}");
            throw;
        }
    }

    [HttpPost("AddOrUpdateItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddOrUpdateItem(
        Guid productId, 
        int quantity, 
        CancellationToken cancellationToken,
        CurrencyTypes currencyType = CurrencyTypes.CAD)
    {
        try
        {
            return HandleResult(await _shoppingCartManager.AddProductToCartAsync(productId, quantity, cancellationToken, currencyType));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to add or update the item inside the shopping cart. Error: {e.Message}");
            throw;
        }
    }
    
    [HttpDelete("Delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid cartId)
    {
        try
        {
            return HandleResult(await _shoppingCartManager.DeleteCartAsync(cartId));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to delete the shopping cart. Error: {e.Message}");
            throw;
        }
    }
    
    [HttpDelete("DeleteItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteItem(Guid productId, Guid cartId)
    {
        try
        {
            return HandleResult(await _shoppingCartManager.DeleteItemInCartAsync(cartId, productId));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to delete the item from the shopping cart. Error: {e.Message}");
            throw;
        }
    }
}