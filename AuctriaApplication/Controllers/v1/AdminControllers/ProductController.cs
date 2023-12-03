using Auctria_Application.Attribute;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using AuctriaApplication.Services.Store.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Auctria_Application.Controllers.v1.AdminControllers;

public class ProductController : BaseAdminController
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

    [HttpPost("CreateProduct")]
    [RequiredPermission(PermissionAction.Product_Create)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateProduct(Guid categoryId, ProductDto productDto, CancellationToken cancellationToken)
    {
        try
        {
            return HandleResult(await _productManager.CreateProductAsync(categoryId, productDto, cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to create the product. Error: {e.Message}");
            throw;
        }
    }

    [HttpPut("UpdateProduct")]
    [RequiredPermission(PermissionAction.Product_Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProduct(Guid productId, ProductDto productDto, CancellationToken cancellationToken)
    {
        try
        {
            return HandleResult(await _productManager.UpdateProductAsync(productId, productDto, cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to update the product. Error: {e.Message}");
            throw;
        }
    }

    [HttpDelete("DeleteProduct")]
    [RequiredPermission(PermissionAction.Product_Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            return HandleResult(await _productManager.ToggleDeleteProductAsync(productId, cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while deleting the product. Error: {e.Message}");
            throw;
        }
    }
}