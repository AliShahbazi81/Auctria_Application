using AuctriaApplication.Domain.Dto;
using AuctriaApplication.Domain.Dto.ViewModel;

namespace AuctriaApplication.Services.Store.Services.Abstract;

/// <summary>
/// Provides methods for managing categories including retrieval, addition, updating, and deletion.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Retrieves a specific category by ID or name.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <param name="categoryId">The unique identifier for the category. Optional.</param>
    /// <param name="categoryName">The name of the category. Optional.</param>
    /// <returns>The category model if found; otherwise, null.</returns>
    Task<CategoryViewModel> GetAsync(
        CancellationToken cancellationToken,
        Guid? categoryId = null,
        string? categoryName = null);

    /// <summary>
    /// Retrieves a list of categories, optionally filtered by name.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <param name="categoryName">The name to filter categories by. Optional.</param>
    /// <returns>A list of category models.</returns>
    Task<IEnumerable<CategoryViewModel>> GetListAsync(
        CancellationToken cancellationToken,
        string? categoryName = null);

    /// <summary>
    /// Adds a new category.
    /// </summary>
    /// <param name="creatorId">The ID of the user creating the category.</param>
    /// <param name="categoryDto">The data transfer object containing category details.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The added category model.</returns>
    Task<CategoryViewModel> AddAsync(
        Guid creatorId,
        CategoryDto categoryDto,
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="categoryId">The unique identifier for the category to be updated.</param>
    /// <param name="updatedDto">The updated category data.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The updated category model.</returns>
    Task<CategoryViewModel> UpdateAsync(
        Guid categoryId,
        CategoryDto updatedDto,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a specific category by ID.
    /// </summary>
    /// <param name="categoryId">The unique identifier for the category to be deleted.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>True if the operation was successful; otherwise, false.</returns>
    Task<bool> DeleteAsync(
        Guid categoryId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Check if category is available
    /// </summary>
    /// <param name="categoryId">The unique identifier for the product. Optional</param>
    /// <param name="categoryName">The name of the category. Optional</param>
    /// <returns>True if the category exists, false otherwise.</returns>
    Task<bool> IsCategoryAsync(
        Guid? categoryId = null, 
        string? categoryName = null);
}