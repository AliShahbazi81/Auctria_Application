using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Infrastructure.Services;
using AuctriaApplication.Infrastructure.Store.Guards;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using AuctriaApplication.Services.Membership.Services.Users.Abstract;
using AuctriaApplication.Services.Store.Dto;
using AuctriaApplication.Services.Store.Dto.ViewModel;
using AuctriaApplication.Services.Store.Services.Abstract;

namespace AuctriaApplication.Infrastructure.Store.Services;

public class CategoryManager : ICategoryManager
{
    private readonly ICategoryService _categoryService;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserService _userService;

    public CategoryManager(
        ICategoryService categoryService, 
        IUserAccessor userAccessor, 
        IUserService userService)
    {
        _categoryService = categoryService;
        _userAccessor = userAccessor;
        _userService = userService;
    }

    public async Task<Result<CategoryViewModel>> GetCategoryAsync(
        CancellationToken cancellationToken,
        Guid? categoryId = null,
        string? categoryName = null)
    {
        // Check if both of the inputs are null
        if (!GeneralGuards.AreInputsNull(categoryId, categoryName))
            return Result<CategoryViewModel>.Failure("Sorry but you have to enter either id or name for getting a category!");
        
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<CategoryViewModel>.Failure("Sorry, but your account is locked.");
        
        var category = await _categoryService.GetAsync(cancellationToken, categoryId, categoryName);
        
        return Result<CategoryViewModel>.Success(category);
    }

    public async Task<Result<IEnumerable<CategoryViewModel>>> GetCategoryListAsync(
        CancellationToken cancellationToken, 
        string categoryName)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<IEnumerable<CategoryViewModel>>.Failure("Sorry, but your account is locked.");

        var category = await _categoryService.GetListAsync(cancellationToken, categoryName);
        
        return Result<IEnumerable<CategoryViewModel>>.Success(category);
    }

    public async Task<Result<CategoryViewModel>> CreateCategoryAsync(
        CategoryDto categoryDto, 
        CancellationToken cancellationToken)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<CategoryViewModel>.Failure("Sorry, but your account is locked.");
        
        // Check if category with the same name exists
        if(await _categoryService.IsCategoryAsync(categoryName: categoryDto.Name))
            return Result<CategoryViewModel>.Failure("It seems we already have a category with the same name in the system.");
        
        // Create the category
        var category = await _categoryService.AddAsync(_userAccessor.GetUserId(), categoryDto, cancellationToken);
        
        return Result<CategoryViewModel>.Success(category);
    }

    public async Task<Result<CategoryViewModel>> UpdateCategoryAsync(
        CategoryDto updatedDto, 
        CancellationToken cancellationToken)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<CategoryViewModel>.Failure("Sorry, but your account is locked.");
        
        // Check if category with the same name exists
        if(await _categoryService.IsCategoryAsync(categoryName: updatedDto.Name))
            return Result<CategoryViewModel>.Failure("It seems we already have a category with the same name in the system.");
        
        // Create the category
        var category = await _categoryService.UpdateAsync(_userAccessor.GetUserId(), updatedDto, cancellationToken);
        
        return Result<CategoryViewModel>.Success(category);
    }

    public async Task<Result<string>> DeleteCategoryAsync(
        Guid categoryId, 
        CancellationToken cancellationToken)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<string>.Failure("Sorry, but your account is locked.");

        var deleteCategory = await _categoryService.DeleteAsync(categoryId, cancellationToken);
        
        return !deleteCategory 
            ? Result<string>.Failure("Sorry but we could not delete the category you wished to delete!") 
            : Result<string>.Success("The category has been deleted successfully!");
    }
}