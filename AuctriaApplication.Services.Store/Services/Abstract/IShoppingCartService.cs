using AuctriaApplication.DataAccess.Entities.Stores;
using AuctriaApplication.Domain.Dto.ViewModel;

namespace AuctriaApplication.Services.Store.Services.Abstract;

/// <summary>
/// Provides services for managing shopping carts, including retrieval, updating, and item management.
/// </summary>
public interface IShoppingCartService
{
    /// <summary>
    /// Asynchronously retrieves a shopping cart for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the shopping cart view model.</returns>
    Task<Cart?> GetAsync(
        Guid userId,
        Guid cartId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a list of shopping carts for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of shopping cart view models.</returns>
    Task<IEnumerable<ShoppingCartViewModel>> GetListAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously calculates the total cost of a shopping cart.
    /// </summary>
    /// <param name="shoppingCartId">The unique identifier of the shopping cart.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total cost of the shopping cart.</returns>
    Task<decimal> GetCostAsync(Guid shoppingCartId);
    
    /// <summary>
    /// Asynchronously adds or updates a product in a shopping cart.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="quantity">The quantity of the product.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the operation was successful.</returns>
    Task<bool> AddOrUpdateProductInCartAsync(
        Guid cartId, 
        Guid productId, 
        int quantity, 
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a shopping cart for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the shopping cart if found, null otherwise.</returns>
    Task<Cart?> GetCartForUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously creates a shopping cart for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created shopping cart.</returns>
    Task<Cart> CreateCartForUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously updates the total cost of a shopping cart.
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the update was successful.</returns>
    Task UpdateCartTotalAsync(
        Guid cartId,
        CancellationToken cancellationToken);

    Task<bool> DeleteCartAsync(
        Guid userId,
        Guid cartId);

    Task<bool> DeleteItemInCartAsync(
        Guid userId,
        Guid cartId,
        Guid productId);

    /// <summary>
    /// Asynchronously checks if items in a shopping cart are reduced and returns a dictionary of low quantity products.
    /// </summary>
    /// <param name="shoppingCartId">The unique identifier of the shopping cart.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple indicating whether the items are reduced and a dictionary of low quantity products.</returns>
    Task<(bool, Dictionary<string, int>?)> AreItemsReducedAsync(Guid shoppingCartId);

    /// <summary>
    /// Asynchronously checks if a shopping cart exists.
    /// </summary>
    /// <param name="shoppingCartId">The unique identifier of the shopping cart.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the shopping cart exists.</returns>
    Task<bool> IsShoppingCartAsync(Guid shoppingCartId);

    /// <summary>
    /// Converts a Cart entity to a ShoppingCartViewModel.
    /// </summary>
    /// <param name="cart">The Cart entity to convert.</param>
    /// <returns>The ShoppingCartViewModel.</returns>
    ShoppingCartViewModel ToViewModel(Cart cart);

    Task<bool?> IsCartPaidAsync(Guid cartId);

}