using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace Auctria_Application.Controllers.v1.StoreControllers;

public class CategoryController : BaseApiController
{
    private readonly ICategoryManager _categoryManager;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(
        ICategoryManager categoryManager, 
        ILogger<CategoryController> logger)
    {
        _categoryManager = categoryManager;
        _logger = logger;
    }

    [HttpGet("Category")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategory(
        CancellationToken cancellationToken, 
        Guid? categoryId = null, 
        string? categoryName = null)
    {
        try
        {
            return HandleResult(await _categoryManager.GetCategoryAsync(cancellationToken, categoryId, categoryName));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to get the category. Error: {e.Message}");
            throw;
        }
    }

    [HttpGet("CategoryList")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategoryList(
        CancellationToken cancellationToken, 
        string? categoryName = null)
    {
        try
        {
            return HandleResult(await _categoryManager.GetCategoryListAsync(cancellationToken, categoryName));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to get the categories list. Error: {e.Message}");
            throw;
        }
    }
}