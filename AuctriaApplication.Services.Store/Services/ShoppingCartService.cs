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

    public async Task<Cart?> GetAsync(
        Guid userId,
        Guid cartId,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var cart = await dbContext.Carts
            .Include(x => x.Payment)
            .Include(x => x.ProductCarts)
            .ThenInclude(x => x.Product)
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                    x.Id == cartId &&
                    x.UserId == userId,
                cancellationToken: cancellationToken);

        return cart;
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

    public async Task<IEnumerable<ShoppingCartViewModel>> GetListAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var carts = await dbContext.Carts
            .Include(x => x.Payment)
            .Include(x => x.ProductCarts)
            .ThenInclude(x => x.Product)
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
            if (quantity > 0) 
            {
                dbContext.ProductCarts.Add(new ProductCart
                {
                    CartId = cartId, 
                    ProductId = productId, 
                    Quantity = quantity
                });
            }
        }
        else
        {
            if (quantity > 0)
            {
                productCart.Quantity = quantity;
            }
            else
            {
                // Remove the product from the cart if quantity is 0
                dbContext.ProductCarts.Remove(productCart);
                
                await dbContext.SaveChangesAsync(CancellationToken.None);

                // Check if the cart is empty and remove it if necessary
                await CheckAndRemoveEmptyCartAsync(cartId);
            }
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);
        return true;
    }
    
    
    public async Task<Cart> CreateCartForUserAsync(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);
        
        var newCart = new Cart
        {
            Total = 0,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
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
    
    public async Task<bool> DeleteCartAsync(
        Guid userId,
        Guid cartId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        var cart = await dbContext.Carts
            .Include(c => c.ProductCarts)
            .FirstOrDefaultAsync(c => 
                c.Id == cartId && 
                c.UserId == userId);

        if (cart == null) 
            return false;
        
        dbContext.Carts.Remove(cart);
        await dbContext.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteItemInCartAsync(
        Guid userId,
        Guid cartId, 
        Guid productId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        var productCart = await dbContext.ProductCarts
            .Include(pc => pc.Cart)
            .FirstOrDefaultAsync(pc => 
                pc.CartId == cartId && 
                pc.ProductId == productId && 
                pc.Cart.UserId == userId);

        if (productCart == null) 
            return false;
        
        dbContext.ProductCarts.Remove(productCart);
        await dbContext.SaveChangesAsync();
        return true;
    }
    
    public ShoppingCartViewModel ToViewModel(Cart cart)
    {
        return new ShoppingCartViewModel
        {
            Id = cart.Id,
            Total = cart.Total.ToString("N2"),
            PaymentStatus = cart.Payment != null! ? cart.Payment.PaymentStatus.ToString() : PaymentStatus.Pending.ToString(),
            CreatedAt = Convert.ToDateTime(cart.CreatedAt.ToLocalTime()
                .ToString("G")),
            Products = cart.ProductCarts.Select(pc => new ProductCartItemViewModel
            {
                ProductId = pc.ProductId,
                Name = pc.Product.Name,
                ImageUrl = pc.Product.ImageUrl ?? string.Empty,
                Price = pc.Product.Price,
                Quantity = pc.Quantity
            }).ToList()
        };
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

    public async Task<bool?> IsCartPaidAsync(Guid cartId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        var cart = await dbContext.Carts
            .AsNoTracking()
            .Include(x => x.Payment)
            .Select(x => new
            {
                x.Id,
                PaymentStatus = (PaymentStatus?)x.Payment.PaymentStatus
            })
            .SingleOrDefaultAsync(x => x.Id == cartId);

        if (cart is null)
            return null;
        
        return cart.PaymentStatus == PaymentStatus.Succeeded;
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
    
    private async Task CheckAndRemoveEmptyCartAsync(Guid cartId)
    {
        await using var dbContext = await _context.CreateDbContextAsync();
        
        // Check if the cart has any other items left
        if (!await dbContext.ProductCarts.AnyAsync(pc => pc.CartId == cartId))
        {
            // If not, remove the cart as well
            var cart = await dbContext.Carts.FindAsync(cartId);
            if (cart != null)
            {
                dbContext.Carts.Remove(cart);
                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
        }
    }
}