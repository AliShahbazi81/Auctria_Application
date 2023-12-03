using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Services.Store.Dto;
using AuctriaApplication.Services.Store.Dto.ViewModel;

namespace AuctriaApplication.Infrastructure.Store.Services.Abstract;

/// <summary>
/// Manages product-related operations including retrieval, addition, updating, and deletion.
/// </summary>
public interface IProductManager
{
    /// <summary>
    /// Asynchronously retrieves a specific product by its ID or name.
    /// </summary>
    /// <param name="productId">The unique identifier for the product. Optional.</param>
    /// <param name="productName">The name of the product. Optional.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the product view model if found.</returns>
    Task<Result<ProductViewModel>> GetProductAsync(
        Guid? productId,
        string? productName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a list of products, optionally filtered by various criteria.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <param name="filterDto">The data transfer object containing filter criteria for products.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of product view models.</returns>
    Task<Result<IEnumerable<ProductViewModel>>> GetProductsListAsync(
        CancellationToken cancellationToken,
        ProductFilterDto filterDto);
    
    /// <summary>
    /// Asynchronously adds a new product.
    /// </summary>
    /// <param name="categoryId">The category ID of the product.</param>
    /// <param name="productDto">The data transfer object containing product details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added product view model.</returns>
    Task<Result<ProductViewModel>> CreateProductAsync(
        Guid categoryId,
        ProductDto productDto,
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously updates an existing product.
    /// </summary>
    /// <param name="productId">The unique identifier for the product to be updated.</param>
    /// <param name="productDto">The updated product data.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated product view model.</returns>
    Task<Result<ProductViewModel>> UpdateProductAsync(
        Guid productId,
        ProductDto productDto,
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously toggles the deletion state of a product.
    /// </summary>
    /// <param name="productId">The unique identifier for the product.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the product is now marked as deleted.</returns>
    Task<Result<string>> ToggleDeleteProductAsync(
        Guid productId,
        CancellationToken cancellationToken);
}