using AuctriaApplication.Domain.Dto.ViewModel;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Infrastructure.Results;

namespace AuctriaApplication.Infrastructure.Store.Services.Abstract;

/// <summary>
/// Manages shopping cart operations such as adding products, updating quantities, and calculating costs.
/// </summary>
public interface IShoppingCartManager
{

    Task<Result<ShoppingCartViewModel?>> GetUserCartAsync(
        Guid cartId,
        CancellationToken cancellationToken, 
        Guid? userId = null,
        CurrencyTypes currencyType = CurrencyTypes.CAD);

    Task<Result<IEnumerable<ShoppingCartViewModel>>> GetUserCartsAsync(
        CancellationToken cancellationToken, 
        Guid? userId = null, 
        CurrencyTypes currencyType = CurrencyTypes.CAD);

    /// <summary>
    /// Asynchronously adds a product to a shopping cart.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to add.</param>
    /// <param name="quantity">The quantity of the product to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <param name="currencyType"></param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated shopping cart view model.</returns>
    Task<Result<ShoppingCartViewModel?>> AddProductToCartAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken,
        CurrencyTypes currencyType = CurrencyTypes.CAD);

    Task<Result<string>> DeleteCartAsync(Guid cartId);

    Task<Result<ShoppingCartViewModel?>> DeleteItemInCartAsync(
        Guid cartId,
        Guid productId);
}