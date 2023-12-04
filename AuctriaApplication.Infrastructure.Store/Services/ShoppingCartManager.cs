using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Infrastructure.Services.Abstract;
using AuctriaApplication.Infrastructure.Store.Guards;
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
    
    public async Task<Result<ShoppingCartViewModel?>> GetUserCartAsync(
        Guid cartId,
        CancellationToken cancellationToken, 
        Guid? userId = null)
    {
        // Check if user is locked or has any restrictions
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            throw new Exception("User account is locked.");
        
        var targetUserId = userId ?? _userAccessor.GetUserId();

        // Get cart
        var cart = await _shoppingCartService.GetAsync(targetUserId, cartId, cancellationToken);
        if (cart is null)
            return Result<ShoppingCartViewModel?>.Failure("It seems we faced a problem retrieving the cart.");

        // Generate and return the CartViewModel
        var cartViewModel = _shoppingCartService.ToViewModel(cart);

        return Result<ShoppingCartViewModel?>.Success(cartViewModel);
    }
    
    public async Task<Result<IEnumerable<ShoppingCartViewModel>>> GetUserCartsAsync(
        CancellationToken cancellationToken, 
        Guid? userId = null)
    {
        // Check if user is locked or has any restrictions
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            throw new Exception("User account is locked.");
        
        var targetUserId = userId ?? _userAccessor.GetUserId();

        // Get or create cart
        var carts = await _shoppingCartService.GetListAsync(targetUserId, cancellationToken);
        
        return Result<IEnumerable<ShoppingCartViewModel>>.Success(carts);
    }

    public async Task<Result<ShoppingCartViewModel?>> AddProductToCartAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        
        // Check if quantity is valid
        if(!GeneralGuards.IsNumberMoreThanZero(quantity))
            return Result<ShoppingCartViewModel?>.Failure("Sorry, but the quantity of the product should be zero or more.");
        
        // Check if user is locked or has any restrictions
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<ShoppingCartViewModel?>.Failure("User account is locked.");

        // Check if the product exists and available
        var product = await _productService.GetProductByIdAsync(productId);
        if (product.Quantity < quantity)
            return Result<ShoppingCartViewModel?>.Failure("Sorry, but the number of item you selected is not currently in stock.");

        // Get the existing cart, if any
        var cart = await _shoppingCartService.GetCartForUserAsync(_userAccessor.GetUserId(), cancellationToken);

        // If quantity is 0 and there is no existing cart, return null
        if (quantity == 0 && cart == null)
            return Result<ShoppingCartViewModel?>.Failure("Sorry, but the quantity of the selected product should be more than zero!");

        // Create cart if it doesn't exist
        cart ??= await _shoppingCartService.CreateCartForUserAsync(_userAccessor.GetUserId(), cancellationToken);

        // Add or update product in cart
        var productCartUpdateResult = await _shoppingCartService.AddOrUpdateProductInCartAsync(
            cart.Id, productId, quantity, cancellationToken);

        if (!productCartUpdateResult)
            return Result<ShoppingCartViewModel?>.Failure("Failed to update the cart.");

        // Calculate the total price of the cart
        await _shoppingCartService.UpdateCartTotalAsync(cart.Id, cancellationToken);
    
        // Get the updated cart
        var latestCart = await _shoppingCartService.GetAsync(_userAccessor.GetUserId(), cart.Id, cancellationToken);

        if (latestCart is null)
            return Result<ShoppingCartViewModel?>.Success(null);

        // Generate and return the updated CartViewModel
        var updatedCartViewModel = _shoppingCartService.ToViewModel(latestCart);

        return Result<ShoppingCartViewModel?>.Success(updatedCartViewModel);
    }

    public async Task<Result<string>> DeleteCartAsync(Guid cartId)
    {
        // Check if user is locked or has any restrictions
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<string>.Failure("User account is locked.");
        
        // Check the cart is paid
        var isPaid = await _shoppingCartService.IsCartPaidAsync(cartId);
        if (isPaid is true)
            return Result<string>.Failure("Sorry, but you cannot delete a paid cart.");

        // Delete cart
        var deleteResult = await _shoppingCartService.DeleteCartAsync(_userAccessor.GetUserId(), cartId);

        return !deleteResult 
            ? Result<string>.Failure("We could not find the cart. It's either deleted or does not exist.") 
            : Result<string>.Success("Cart deleted successfully.");
    }
    
    public async Task<Result<ShoppingCartViewModel?>> DeleteItemInCartAsync(
        Guid cartId,
        Guid productId)
    {
        // Check the cart is paid
        var isPaid = await _shoppingCartService.IsCartPaidAsync(cartId);
        if (isPaid is true)
            return Result<ShoppingCartViewModel?>.Failure("Sorry, but you cannot delete the item in a paid cart.");

        // Delete item in cart
        var deleteResult = await _shoppingCartService.DeleteItemInCartAsync(
            _userAccessor.GetUserId(), cartId, productId);
        
        // Calculate the total price of the cart
        await _shoppingCartService.UpdateCartTotalAsync(cartId, CancellationToken.None);
        
        // Get the updated cart
        var latestCart = await _shoppingCartService.GetAsync(_userAccessor.GetUserId(), cartId, CancellationToken.None);

        if (latestCart is null)
            return Result<ShoppingCartViewModel?>.Success(null);
        
        // Generate and return the updated CartViewModel
        var updatedCartViewModel = _shoppingCartService.ToViewModel(latestCart);
        
        return !deleteResult 
            ? Result<ShoppingCartViewModel?>.Failure("We could not find the item in the cart. It's either deleted or does not exist.") 
            : Result<ShoppingCartViewModel?>.Success(updatedCartViewModel);
    }
}