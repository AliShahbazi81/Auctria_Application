using AuctriaApplication.DataAccess.DbContext;
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
}