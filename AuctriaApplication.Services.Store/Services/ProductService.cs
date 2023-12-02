using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.DataAccess.Entities.Stores;
using AuctriaApplication.Domain.Helper;
using AuctriaApplication.Services.Store.Dto;
using AuctriaApplication.Services.Store.Dto.ViewModel;
using AuctriaApplication.Services.Store.Exceptions;
using AuctriaApplication.Services.Store.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AuctriaApplication.Services.Store.Services;

public class ProductService : IProductService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public ProductService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<ProductViewModel> GetAsync(
        CancellationToken cancellationToken,
        Guid? productId = null,
        string? productName = null)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var product = productId is null
            ? await dbContext.Products
                .AsNoTracking()
                .Where(x => EF.Functions.Like(x.Name, $"%{productName}%"))
                .Include(x => x.Category)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken)
            : await dbContext.Products
                .AsNoTracking()
                .Where(x => x.Id == productId)
                .Include(x => x.Category)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);

        if (product is null)
            throw new NotFoundException();

        return ToViewModel(product);
    }

    public async Task<IEnumerable<ProductViewModel>> GetListAsync(
        CancellationToken cancellationToken,
        string? productName = null,
        string? categoryName = null,
        double minPrice = 0,
        double maxPrice = 0,
        int pageNumber = 1,
        int pageSize = 20, 
        bool isDeleted = false)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        productName = StringHelper.ConvertToLowerCaseNoSpaces(productName);
        categoryName = StringHelper.ConvertToLowerCaseNoSpaces(categoryName);

        var products = await dbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => productName == null || EF.Functions.Like(x.Name.ToLower().Replace(" ", ""), productName) &&
                        categoryName == null || EF.Functions.Like(x.Category.Name.ToLower().Replace(" ", ""), categoryName) &&
                        x.Price >= (decimal)minPrice &&
                        x.Price <= (decimal)maxPrice &&
                        x.IsDeleted == isDeleted)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return products.Select(ToViewModel);
    }
    
    public async Task<ProductViewModel> AddAsync(
        Guid creatorId,
        Guid categoryId,
        ProductDto productDto,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var newProduct = dbContext.Add(new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            ImageUrl = productDto.ImageUrl,
            Quantity = productDto.Quantity,
            Price = (decimal)productDto.Price,
            CategoryId = categoryId,
            AddedBy = creatorId
        }).Entity;

        var isSaved = await dbContext.SaveChangesAsync(cancellationToken) > 0;

        if (!isSaved)
            throw new NotSavedException(true);

        return ToViewModel(newProduct);
    }
    
    public async Task<ProductViewModel> UpdateAsync(
        Guid productId,
        ProductDto productDto,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var product = await dbContext.Products
            .SingleOrDefaultAsync(x => x.Id == productId, cancellationToken: cancellationToken);

        if (product is null)
            throw new NotFoundException();

        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.ImageUrl = productDto.ImageUrl;
        product.Quantity = productDto.Quantity;
        product.Price = (decimal)productDto.Price;

        var isSaved = await dbContext.SaveChangesAsync(cancellationToken) > 0;
        
        if(!isSaved)
            throw new NotSavedException(false);

        return ToViewModel(product);
    }
    
    public async Task<bool> ToggleDeleteAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var product = await dbContext.Products
            .SingleOrDefaultAsync(x => x.Id == productId, cancellationToken: cancellationToken);

        if (product is null)
            throw new NotFoundException();

        product.IsDeleted = !product.IsDeleted;

        await dbContext.SaveChangesAsync(cancellationToken);

        return product.IsDeleted;
    }

    private static ProductViewModel ToViewModel(Product product)
    {
        return new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            CategoryName = product.Category.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            ImageUrl = product.ImageUrl,
            Price = product.Price.ToString("N0"),
            IsDeleted = product.IsDeleted
        };
    }
}