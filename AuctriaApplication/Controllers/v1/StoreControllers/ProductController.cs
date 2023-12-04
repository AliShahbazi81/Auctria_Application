using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Auctria_Application.Controllers.v1.StoreControllers;

public class ProductController : BaseApiController
{
    private readonly IProductManager _productManager;
    private readonly ILogger<ProductController> _logger;

    public ProductController(
        IProductManager productManager,
        ILogger<ProductController> logger)
    {
        _productManager = productManager;
        _logger = logger;
    }

    [HttpGet("GetProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProduct(
        Guid? productId, 
        string? productName, 
        CancellationToken cancellationToken, 
        CurrencyTypes currencyType = CurrencyTypes.CAD)
    {
        try
        {
            return HandleResult(await _productManager.GetProductAsync(productId, productName, cancellationToken, currencyType));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to retrieve the product. Error: {e.Message}");
            throw;
        }
    }

    [HttpGet("GetProducts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProducts(
        CancellationToken cancellationToken,
        [FromQuery] string? productName = null,
        [FromQuery] string? categoryName = null,
        [FromQuery] double? minPrice = null,
        [FromQuery] double? maxPrice = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool isDeleted = false,
        [FromQuery] CurrencyTypes currencyType = CurrencyTypes.CAD)
    {
        try
        {
            return HandleResult(await _productManager.GetProductsListAsync(
                cancellationToken,
                productName,
                categoryName,
                minPrice,
                maxPrice,
                pageNumber,
                pageSize,
                isDeleted,
                currencyType));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to get the list of products. Error: {e.Message}");
            throw;
        }
    }
}