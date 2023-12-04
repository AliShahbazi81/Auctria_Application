using Auctria_Application.Attribute;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using AuctriaApplication.Services.Store.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Auctria_Application.Controllers.v1.AdminControllers;

public class CategoryController : BaseAdminController
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

    [HttpPost("Create")]
    [RequiredPermission(PermissionAction.Category_Create)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        CategoryDto categoryDto, 
        CancellationToken cancellationToken)
    {
        try
        {
            return HandleResult(await _categoryManager.CreateCategoryAsync(categoryDto, cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while creating the category. Error: {e.Message}");
            throw;
        }
    }

    [HttpPut("Update")]
    [RequiredPermission(PermissionAction.Category_Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(Guid categoryId, CategoryDto updatedDto, CancellationToken cancellationToken)
    {
        try
        {
            return HandleResult(await _categoryManager.UpdateCategoryAsync(categoryId, updatedDto, cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while trying to update the category. Error: {e.Message}");
            throw;
        }
    }

    [HttpDelete("Delete")]
    [RequiredPermission(PermissionAction.Category_Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid categoryId, CancellationToken cancellationToken)
    {
        try
        {
            return HandleResult(await _categoryManager.DeleteCategoryAsync(categoryId, cancellationToken));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error while deleting the category. Error: {e.Message}");
            throw;
        }
    }
}