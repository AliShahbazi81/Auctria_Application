using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Services.Store.Dto;
using AuctriaApplication.Services.Store.Dto.ViewModel;

namespace AuctriaApplication.Infrastructure.Store.Services.Abstract;

public interface IProductManager
{
    Task<Result<ProductViewModel>> GetProductAsync(
        Guid? productId,
        string? productName,
        CancellationToken cancellationToken);

    Task<Result<IEnumerable<ProductViewModel>>> GetProductsListAsync(
        CancellationToken cancellationToken,
        ProductFilterDto filterDto);

    Task<Result<ProductViewModel>> CreateProductAsync(
        Guid categoryId,
        ProductDto productDto,
        CancellationToken cancellationToken);

    Task<Result<ProductViewModel>> UpdateProductAsync(
        Guid productId,
        ProductDto productDto,
        CancellationToken cancellationToken);

    Task<Result<string>> ToggleDeleteProductAsync(
        Guid productId,
        CancellationToken cancellationToken);
}