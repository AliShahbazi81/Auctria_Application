using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.DataAccess.Entities.Stores;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Services.Store.Dto.ViewModel;
using AuctriaApplication.Services.Store.Exceptions;
using AuctriaApplication.Services.Store.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AuctriaApplication.Services.Store.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public ShoppingCartService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<ShoppingCartViewModel> GetAsync(
        Guid userId,
        Guid cartId,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var cart = await dbContext.Carts
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                    x.Id == cartId &&
                    x.UserId == userId,
                cancellationToken: cancellationToken);

        if (cart is null)
            throw new NotFoundException();

        return ToViewModel(cart);
    }

    public async Task<decimal> GetCostAsync(Guid shoppingCartId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        return await dbContext.Carts
            .AsNoTracking()
            .Where(x => x.Id == shoppingCartId)
            .Select(x => x.Total)
            .SingleAsync();
    }

    public async Task<IEnumerable<ShoppingCartViewModel>> GetListAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var carts = await dbContext.Carts
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken: cancellationToken);

        return carts.Select(ToViewModel);
    }
    
    public async Task<bool> AddOrUpdateProductInCartAsync(
        Guid cartId, 
        Guid productId, 
        int quantity, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);
        
        var productCart = await dbContext.ProductCarts
            .FirstOrDefaultAsync(pc => 
                pc.CartId == cartId && 
                pc.ProductId == productId, 
                cancellationToken);

        if (productCart == null)
        {
            dbContext.ProductCarts.Add(new ProductCart
            {
                CartId = cartId, 
                ProductId = productId, 
                Quantity = quantity
            });
        }
        else
        {
            productCart.Quantity = quantity; 
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    public async Task<Cart?> GetCartForUserAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);
        return await dbContext.Carts
            .Include(x => x.Payment)
            .FirstOrDefaultAsync(c => 
                c.UserId == userId && 
                c.Payment.PaymentStatus != PaymentStatus.Succeeded, 
                cancellationToken);
    }

    public async Task<Cart> CreateCartForUserAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);
        var newCart = new Cart
        {
            UserId = userId,
            Total = 0
        };
        dbContext.Carts.Add(newCart);
        await dbContext.SaveChangesAsync(cancellationToken);
        return newCart;
    }

    public async Task UpdateCartTotalAsync(
        Guid cartId, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);
        var cart = await dbContext.Carts
            .Include(c => c.ProductCarts)
            .ThenInclude(pc => pc.Product)
            .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);

        if (cart != null)
        {
            cart.Total = cart.ProductCarts.Sum(pc => pc.Quantity * pc.Product.Price);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    public async Task<(bool, Dictionary<string, int>?)> AreItemsReducedAsync(Guid shoppingCartId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        
        var cart = await dbContext.Carts
            .Include(c => c.ProductCarts)
            .ThenInclude(pc => pc.Product)
            .FirstOrDefaultAsync(c => c.Id == shoppingCartId);

        if (cart == null) 
            return (true, null);
        
        foreach (var productCart in cart.ProductCarts)
        {
            // If quantity is more than available, throw exception
            if (productCart.Quantity > productCart.Product.Quantity)
                throw new NotEnoughQuantityException(productCart.Product.Name);
            
            productCart.Product.Quantity -= productCart.Quantity;
        }
        
        var isSaved = await dbContext.SaveChangesAsync() > 0;

        if (!isSaved)
            return (false, null);
        
        // Get the items which quantities are less than 10
        var items = await GetItemsAsync(shoppingCartId);
        var lowQuantityProducts = 
            items
                .Where(item => item.Quantity <= 10)
                .ToDictionary(item => item.Name, item => item.Quantity);

        // Return both the reduction status and the dictionary of low quantity products
        return (true, lowQuantityProducts);
    }

    public async Task<bool> IsShoppingCartAsync(Guid shoppingCartId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var shoppingCart = await dbContext.Carts
            .AsNoTracking()
            .Select(x => new
            {
                x.Id
            })
            .AnyAsync(x =>
                x.Id == shoppingCartId);

        return shoppingCart;
    }
    
    public ShoppingCartViewModel ToViewModel(Cart cart)
    {
        return new ShoppingCartViewModel
        {
            Id = cart.Id,
            Total = cart.Total.ToString("N0"),
            PaymentStatus = cart.Payment.PaymentStatus.ToString(),
            CreatedAt = Convert.ToDateTime(cart.CreatedAt.ToLocalTime()
                .ToString("G")),
        };
    }

    private async Task<IEnumerable<Product>> GetItemsAsync(Guid shoppingCartId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var shoppingCartItems = await dbContext.Carts
            .AsNoTracking()
            .Include(x => x.ProductCarts)
            .ThenInclude(x => x.Product)
            .Where(x => x.Id == shoppingCartId)
            .SelectMany(x => x.ProductCarts)
            .Select(pc => pc.Product)
            .ToListAsync();

        return shoppingCartItems;
    }
}