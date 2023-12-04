using AuctriaApplication.Domain.Dto;
using AuctriaApplication.Domain.Dto.ViewModel;
using AuctriaApplication.Infrastructure.Results;

namespace AuctriaApplication.Infrastructure.Store.Services.Abstract;

/// <summary>
/// Manages category-related operations such as retrieval, creation, update, and deletion.
/// </summary>
public interface ICategoryManager
{
    /// <summary>
    /// Asynchronously retrieves a specific category by its ID or name.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <param name="categoryId">The unique identifier for the category. Optional.</param>
    /// <param name="categoryName">The name of the category. Optional.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the category view model if found.</returns>
    Task<Result<CategoryViewModel>> GetCategoryAsync(
        CancellationToken cancellationToken,
        Guid? categoryId = null,
        string? categoryName = null);

    /// <summary>
    /// Asynchronously retrieves a list of categories, optionally filtered by name.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <param name="categoryName">The name to filter categories by. Optional.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of category view models.</returns>
    Task<Result<IEnumerable<CategoryViewModel>>> GetCategoryListAsync(
        CancellationToken cancellationToken,
        string? categoryName = null);

    /// <summary>
    /// Asynchronously creates a new category.
    /// </summary>
    /// <param name="categoryDto">The data transfer object containing category details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created category view model.</returns>
    Task<Result<CategoryViewModel>> CreateCategoryAsync(
        CategoryDto categoryDto,
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously updates an existing category.
    /// </summary>
    /// <param name="categoryId">The unique identifier for the category.</param>
    /// <param name="updatedDto">The updated category data.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated category view model.</returns>
    Task<Result<CategoryViewModel>> UpdateCategoryAsync(
        Guid categoryId,
        CategoryDto updatedDto, 
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously deletes a specific category by its ID.
    /// </summary>
    /// <param name="categoryId">The unique identifier for the category to be deleted.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the deletion was successful.</returns>
    Task<Result<string>> DeleteCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken);
}