using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.Services.Store.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AuctriaApplication.Services.Store.Services;

public class CartService : ICartService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public CartService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }
}