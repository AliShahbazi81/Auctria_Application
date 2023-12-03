using System.Xml;
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

    [HttpPost("AddOrUpdateItem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddOrUpdateItem(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        try
        {
            return HandleResult(await _shoppingCartManager.AddProductToCartAsync(productId, quantity, cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to add or update the item inside the shopping cart. Error: {e.Message}");
            throw;
        }
    }
}