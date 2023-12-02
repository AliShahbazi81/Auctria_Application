using AuctriaApplication.Domain.Results;
using AuctriaApplication.Infrastructure.Services;
using AuctriaApplication.Infrastructure.Store.Guards;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using AuctriaApplication.Services.Membership.Services.Users.Abstract;
using AuctriaApplication.Services.Store.Dto;
using AuctriaApplication.Services.Store.Dto.ViewModel;
using AuctriaApplication.Services.Store.Services.Abstract;

namespace AuctriaApplication.Infrastructure.Store.Services;

public class ProductManager : IProductManager
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserService _userService;

    public ProductManager(
        IProductService productService, 
        IUserAccessor userAccessor, 
        IUserService userService, 
        ICategoryService categoryService)
    {
        _productService = productService;
        _userAccessor = userAccessor;
        _userService = userService;
        _categoryService = categoryService;
    }

    public async Task<Result<ProductViewModel>> GetProductAsync(
        Guid? productId, 
        string? productName, 
        CancellationToken cancellationToken)
    {
        // Check if both of the inputs are null
        if (!GeneralGuards.AreInputsNull(productId, productName))
            return Result<ProductViewModel>.Failure("Sorry but you have to enter either id or name for getting a product!");
        
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<ProductViewModel>.Failure("Sorry, but your account is locked.");
        
        var product = await _productService.GetAsync(cancellationToken, productId, productName);
        
        return Result<ProductViewModel>.Success(product);
    }

    public async Task<Result<IEnumerable<ProductViewModel>>> GetProductsListAsync(
        CancellationToken cancellationToken,
        ProductFilterDto filterDto)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<IEnumerable<ProductViewModel>>.Failure("Sorry, but your account is locked.");
        
        // Get the list of Products
        var products = await _productService.GetListAsync(cancellationToken, filterDto);
        
        return Result<IEnumerable<ProductViewModel>>.Success(products);
    }

    public async Task<Result<ProductViewModel>> CreateProductAsync(
        Guid categoryId, 
        ProductDto productDto, 
        CancellationToken cancellationToken)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<ProductViewModel>.Failure("Sorry, but your account is locked.");
        
        // Check if product exists
        if(await _productService.IsProductAsync(productName: productDto.Name))
            return Result<ProductViewModel>.Failure("Sorry, but we already have a product with the same name in the system.");
        
        // Check if category exists
        if(!await _categoryService.IsCategoryAsync(categoryId))
            return Result<ProductViewModel>.Failure("We could find the category selected for the product.");
        
        // Add the product
        var product = await _productService.AddAsync(_userAccessor.GetUserId(), categoryId, productDto, cancellationToken);
        
        return Result<ProductViewModel>.Success(product);
    }

    public async Task<Result<ProductViewModel>> UpdateProductAsync(
        Guid productId, 
        ProductDto productDto, 
        CancellationToken cancellationToken)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<ProductViewModel>.Failure("Sorry, but your account is locked.");
        
        // Check if the productId exists
        if(!await _productService.IsProductAsync(productId))
            return Result<ProductViewModel>.Failure("Sorry, but we could not find the product you are looking for.");
        
        // Check if product name exists
        if(await _productService.IsProductAsync(productName: productDto.Name))
            return Result<ProductViewModel>.Failure("Sorry, but it seems that we already have a product with the same name!");
        
        // Update the product
        var updatedProduct = await _productService.UpdateAsync(productId, productDto, cancellationToken);
        
        return Result<ProductViewModel>.Success(updatedProduct);
    }

    public async Task<Result<string>> ToggleDeleteProductAsync(
        Guid productId, 
        CancellationToken cancellationToken)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<string>.Failure("Sorry, but your account is locked.");
        
        // Check if the productId exists
        if(!await _productService.IsProductAsync(productId))
            return Result<string>.Failure("Sorry, but we could not find the product you are looking for.");
        
        // Soft delete the product
        var deletedProduct = await _productService.ToggleDeleteAsync(productId, cancellationToken);
        
        return deletedProduct 
            ? Result<string>.Success("The product has been deleted successfully!") 
            : Result<string>.Failure("Sorry, but it seems we faced an issue while deleting the product!");
    }
}