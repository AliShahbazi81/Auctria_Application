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
            .FirstOrDefaultAsync(c => 
                c.UserId == userId && 
                c.PaymentStatus == PaymentStatus.Pending, 
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
            PaymentStatus = PaymentStatus.Pending,
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
    
    public ShoppingCartViewModel ToViewModel(Cart cart)
    {
        return new ShoppingCartViewModel
        {
            Id = cart.Id,
            Total = cart.Total.ToString("N0"),
            PaymentStatus = cart.PaymentStatus.ToString(),
            CreatedAt = Convert.ToDateTime(cart.CreatedAt.ToLocalTime()
                .ToString("G")),
        };
    }
}