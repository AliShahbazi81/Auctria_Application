using AuctriaApplication.DataAccess.Entities.Stores;
using AuctriaApplication.Domain.Dto;

namespace AuctriaApplication.Services.Store.Services.Abstract;

/// <summary>
/// Provides services for managing products including retrieval, addition, updating, and deletion.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves a specific product by ID or name.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <param name="productId">The unique identifier for the product. Optional.</param>
    /// <param name="productName">The name of the product. Optional.</param>
    /// <returns>The product model if found; otherwise, throws NotFoundException.</returns>
    Task<IEnumerable<ProductViewModel>> GetAsync(
        CancellationToken cancellationToken,
        Guid? productId = null,
        string? productName = null);

    Task<Product> GetProductByIdAsync(Guid productId);

    /// <summary>
    /// Retrieves a list of products, optionally filtered by various criteria.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <param name="filterDto"></param>
    /// <returns>A list of product models based on the specified filters.</returns>
    Task<IEnumerable<ProductViewModel>> GetListAsync(
        CancellationToken cancellationToken,
        string? productName = null,
        string? categoryName = null,
        double? minPrice = null,
        double? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 20,
        bool isDeleted = false);
    
    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="creatorId">The ID of the user creating the product.</param>
    /// <param name="categoryId">The category ID of the product.</param>
    /// <param name="productDto">The data transfer object containing product details.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The added product model.</returns>
    Task<ProductViewModel> AddAsync(
        Guid creatorId,
        Guid categoryId,
        ProductDto productDto,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="productId">The unique identifier for the product to be updated.</param>
    /// <param name="productDto">The updated product data.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The updated product model.</returns>
    Task<ProductViewModel> UpdateAsync(
        Guid productId,
        ProductDto productDto,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Toggles the deletion state of a product.
    /// </summary>
    /// <param name="productId">The unique identifier for the product.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>True if the product is now marked as deleted, false otherwise.</returns>
    Task<bool> ToggleDeleteAsync(
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Check if product is available
    /// </summary>
    /// <param name="productId">The unique identifier for the product.</param>
    /// <param name="productName">The name of the product. Optional</param>
    /// <returns>True if the product exists, false otherwise.</returns>
    Task<bool> IsProductAsync(
        Guid? productId = null, 
        string? productName = null);
}