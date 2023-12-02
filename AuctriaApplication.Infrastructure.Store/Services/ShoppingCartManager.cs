using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Infrastructure.Services;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using AuctriaApplication.Services.Membership.Services.Users.Abstract;
using AuctriaApplication.Services.Store.Dto.ViewModel;
using AuctriaApplication.Services.Store.Services.Abstract;

namespace AuctriaApplication.Infrastructure.Store.Services;

public class ShoppingCartManager : IShoppingCartManager
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IProductService _productService;
    private readonly IUserService _userService;
    private readonly IUserAccessor _userAccessor;

    public ShoppingCartManager(
        IProductService productService,
        IUserService userService,
        IShoppingCartService shoppingCartService,
        IUserAccessor userAccessor)
    {
        _productService = productService;
        _userService = userService;
        _shoppingCartService = shoppingCartService;
        _userAccessor = userAccessor;
    }

    public async Task<Result<ShoppingCartViewModel>> AddProductToCartAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        // Check if user is locked or has any restrictions
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<ShoppingCartViewModel>.Failure("User account is locked.");

        // Check if the product exists and available
        var product = await _productService.GetProductByIdAsync(productId);
        if (product.Quantity < quantity)
            return Result<ShoppingCartViewModel>.Failure("Sorry, but the number of item you selected is not currently in stock.");

        // Get or create cart
        var cart = await _shoppingCartService.GetCartForUserAsync(_userAccessor.GetUserId(), cancellationToken) ??
                   await _shoppingCartService.CreateCartForUserAsync(_userAccessor.GetUserId(), cancellationToken);

        // Add or update product in cart
        var productCartUpdateResult = await _shoppingCartService.AddOrUpdateProductInCartAsync(
            cart.Id, productId, quantity, cancellationToken);

        if (!productCartUpdateResult)
            return Result<ShoppingCartViewModel>.Failure("Failed to update the cart.");

        // Optionally update the cart total or any other relevant fields
        await _shoppingCartService.UpdateCartTotalAsync(cart.Id, cancellationToken);

        // Generate and return the updated CartViewModel
        var updatedCartViewModel = _shoppingCartService.ToViewModel(cart);

        return Result<ShoppingCartViewModel>.Success(updatedCartViewModel);
    }
}