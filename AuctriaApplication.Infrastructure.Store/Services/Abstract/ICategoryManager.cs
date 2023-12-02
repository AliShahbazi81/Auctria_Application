using AuctriaApplication.Domain.Results;
using AuctriaApplication.Services.Store.Dto;
using AuctriaApplication.Services.Store.Dto.ViewModel;

namespace AuctriaApplication.Infrastructure.Store.Services.Abstract;

public interface ICategoryManager
{
    Task<Result<CategoryViewModel>> GetCategoryAsync(
        CancellationToken cancellationToken,
        Guid? categoryId = null,
        string? categoryName = null);

    Task<Result<IEnumerable<CategoryViewModel>>> GetCategoryListAsync(
        CancellationToken cancellationToken,
        string categoryName);

    Task<Result<CategoryViewModel>> CreateCategoryAsync(
        CategoryDto categoryDto,
        CancellationToken cancellationToken);

    Task<Result<CategoryViewModel>> UpdateCategoryAsync(
        CategoryDto updatedDto,
        CancellationToken cancellationToken);

    Task<Result<string>> DeleteCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken);
}