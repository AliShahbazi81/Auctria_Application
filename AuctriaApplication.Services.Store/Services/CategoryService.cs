using System.Linq.Expressions;
using System.Net.Security;
using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.DataAccess.Entities.Stores;
using AuctriaApplication.Domain.Exceptions;
using AuctriaApplication.Domain.Helper;
using AuctriaApplication.Services.Store.Dto;
using AuctriaApplication.Services.Store.Dto.ViewModel;
using AuctriaApplication.Services.Store.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AuctriaApplication.Services.Store.Services;

public class CategoryService : ICategoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public CategoryService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<CategoryViewModel> GetAsync(
        CancellationToken cancellationToken,
        Guid? categoryId = null,
        string? categoryName = null)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var category = categoryId is null
            ? await dbContext.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(x => 
                    EF.Functions.Like(x.Name, $"%{categoryName}%"), 
                    cancellationToken: cancellationToken)
            : await dbContext.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(x => 
                    x.Id == categoryId, 
                    cancellationToken: cancellationToken);
        
        if (category is null)
            throw new NotFoundException();
        
        return ToViewModel(category);
    }

    public async Task<IEnumerable<CategoryViewModel>> GetListAsync(
        CancellationToken cancellationToken, 
        string? categoryName = null)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);
        
        var categories = string.IsNullOrWhiteSpace(categoryName)
            ? await dbContext.Categories
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken)
            : await dbContext.Categories
                .AsNoTracking()
                .Where(x => EF.Functions.Like(x.Name, $"%{categoryName}%"))
                .ToListAsync(cancellationToken: cancellationToken);
        
        return categories.Select(ToViewModel);
    }

    public async Task<CategoryViewModel> AddAsync(
        Guid creatorId,
        CategoryDto categoryDto, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var newCategory = dbContext.Add(new Category
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description,
            CreatedAt = DateTime.UtcNow,

            UserId = creatorId
        }).Entity;

        var isSaved = await dbContext.SaveChangesAsync(cancellationToken) > 0;

        if (!isSaved)
            throw new NotSavedException(categoryDto.Name);

        return ToViewModel(newCategory);
    }

    public async Task<CategoryViewModel> UpdateAsync(
        Guid categoryId,
        CategoryDto updatedDto, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var category = await dbContext.Categories
            .SingleOrDefaultAsync(x => 
                x.Id == categoryId, 
                cancellationToken: cancellationToken);

        if (category is null)
            throw new NotFoundException();

        category.Name = updatedDto.Name;
        category.Description = updatedDto.Description;

        var isSaved = await dbContext.SaveChangesAsync(cancellationToken) > 0;

        if (!isSaved)
            throw new NotSavedException(false);
        
        return ToViewModel(category);
    }

    public async Task<bool> DeleteAsync(
        Guid categoryId, 
        CancellationToken cancellationToken)
    {
        await using var dbContext = await _context.CreateDbContextAsync(cancellationToken);

        var category = await dbContext.Categories
            .SingleOrDefaultAsync(x =>
                    x.Id == categoryId,
                cancellationToken: cancellationToken);

        if (category is null)
            throw new NotFoundException();

        dbContext.Categories.Remove(category);
        var isSaved = await dbContext.SaveChangesAsync(cancellationToken) > 0;

        if (!isSaved)
            throw new NotSavedException(false);
        
        return true;
    }

    public async Task<bool> IsCategoryAsync(
        Guid? categoryId = null, 
        string? categoryName = null)
    {
        await using var dbContext = await _context.CreateDbContextAsync();

        categoryName = StringHelper.ConvertToLowerCaseNoSpaces(categoryName);

        Expression<Func<Category, bool>> predicate = category =>
            (categoryId.HasValue && category.Id == categoryId.Value) ||
            (!string.IsNullOrWhiteSpace(categoryName) && category.Name.ToLower().Replace(" ", "") == categoryName);

        var categoryExists = await dbContext.Categories
            .AsNoTracking()
            .AnyAsync(predicate);

        return categoryExists;
    }

    private static CategoryViewModel ToViewModel(Category category)
    {
        return new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = Convert.ToDateTime(category.CreatedAt.ToString("yyyy-MM-dd HH:mm")),
        };
    }
}