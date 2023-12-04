using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Infrastructure.Services.Abstract;
using AuctriaApplication.Infrastructure.Store.Guards;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using AuctriaApplication.Services.ExchangeAPI.Services.Abstract;
using AuctriaApplication.Services.Membership.Services.Users.Abstract;
using AuctriaApplication.Services.Redis.Services.Abstract;
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
    private readonly IExchangeService _exchangeService;
    private readonly IRedisService _redisService;

    public ProductManager(
        IProductService productService,
        IUserAccessor userAccessor,
        IUserService userService,
        ICategoryService categoryService,
        IExchangeService exchangeService,
        IRedisService redisService)
    {
        _productService = productService;
        _userAccessor = userAccessor;
        _userService = userService;
        _categoryService = categoryService;
        _exchangeService = exchangeService;
        _redisService = redisService;
    }

    public async Task<Result<IEnumerable<ProductViewModel>>> GetProductAsync(
        Guid? productId,
        string? productName,
        CancellationToken cancellationToken,
        CurrencyTypes currencyType = CurrencyTypes.CAD)
    {
        // Check if both of the inputs are null
        if (!GeneralGuards.AreInputsNull(productId, productName))
            return Result<IEnumerable<ProductViewModel>>.Failure("Sorry but you have to enter either id or name for getting a product!");

        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<IEnumerable<ProductViewModel>>.Failure("Sorry, but your account is locked.");

        // Construct a Redis key based on the product ID or name
        var redisKey = $"product_{productId}";

        List<ProductViewModel> productViewModels;

        // Try to get the product from Redis
        var productFromRedis = await _redisService.GetAsync<List<ProductViewModel>>(redisKey);
        if (productFromRedis != null)
        {
            productViewModels = productFromRedis;
        }
        else
        {
            var product = await _productService.GetAsync(cancellationToken, productId, productName);

            // Cache the product in Redis for future requests
            var viewModels = product.ToList();
            await _redisService.SetAsync(redisKey, viewModels, TimeSpan.FromDays(30));

            productViewModels = viewModels.ToList();
        }

        // Currency conversion logic
        if (currencyType == CurrencyTypes.CAD)
            return Result<IEnumerable<ProductViewModel>>.Success(productViewModels);

        // Get the conversion rate
        var convertedPrice = await _exchangeService.GetConversionRateAsync(currencyType.ToString());

        // Convert the price of each product to the selected currency
        var viewModelsList = productViewModels.ToList();
        foreach (var productViewModel in viewModelsList)
        {
            // Display 2 decimal places
            productViewModel.Price = Math.Round(productViewModel.Price * convertedPrice, 2);
            productViewModel.Currency = currencyType.ToString();
        }

        return Result<IEnumerable<ProductViewModel>>.Success(viewModelsList);
    }

    public async Task<Result<IEnumerable<ProductViewModel>>> GetProductsListAsync(
        CancellationToken cancellationToken,
        string? productName = null,
        string? categoryName = null,
        double? minPrice = null,
        double? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 20,
        bool isDeleted = false,
        CurrencyTypes currencyType = CurrencyTypes.CAD)
    {
        // Check if user is locked
        if (await _userService.IsUserLockedAsync(_userAccessor.GetUserId()))
            return Result<IEnumerable<ProductViewModel>>.Failure("Sorry, but your account is locked.");

        // Get the list of Products
        var products = await _productService.GetListAsync(
            cancellationToken,
            productName,
            categoryName,
            minPrice,
            maxPrice,
            pageNumber,
            pageSize,
            isDeleted);

        // If currency is not CAD, convert the price
        if (currencyType == CurrencyTypes.CAD)
            return Result<IEnumerable<ProductViewModel>>.Success(products);

        // Get the conversion rate
        var convertedPrice = await _exchangeService.GetConversionRateAsync(currencyType.ToString());

        // Convert the price of each product to the selected currency
        var productViewModels = products.ToList();
        foreach (var product in productViewModels)
        {
            // Display 2 decimal places
            product.Price = Math.Round(product.Price * convertedPrice, 2);
            product.Currency = currencyType.ToString();
        }

        return Result<IEnumerable<ProductViewModel>>.Success(productViewModels);
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
        if (await _productService.IsProductAsync(productName: productDto.Name))
            return Result<ProductViewModel>.Failure("Sorry, but we already have a product with the same name in the system.");

        // Check if category exists
        if (!await _categoryService.IsCategoryAsync(categoryId))
            return Result<ProductViewModel>.Failure("We could find the category selected for the product.");

        // Add the product
        var product = await _productService.AddAsync(_userAccessor.GetUserId(), categoryId, productDto, cancellationToken);

        // Cache the new product in Redis
        var redisKey = $"product_{product.Id}";
        await _redisService.SetAsync(redisKey, new List<ProductViewModel> { product }, TimeSpan.FromDays(30));

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
        if (!await _productService.IsProductAsync(productId))
            return Result<ProductViewModel>.Failure("Sorry, but we could not find the product you are looking for.");

        // Check if product name exists
        if (await _productService.IsProductAsync(productName: productDto.Name))
            return Result<ProductViewModel>.Failure("Sorry, but it seems that we already have a product with the same name!");

        // Update the product
        var updatedProduct = await _productService.UpdateAsync(productId, productDto, cancellationToken);

        // Construct a Redis key based on the product ID
        var redisKey = $"product_{productId}";

        // Update or add the product in Redis
        await _redisService.SetAsync(redisKey, new List<ProductViewModel> { updatedProduct }, TimeSpan.FromDays(30));

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
        if (!await _productService.IsProductAsync(productId))
            return Result<string>.Failure("Sorry, but we could not find the product you are looking for.");

        // Soft delete or undelete the product in the primary database
        var deletedProduct = await _productService.ToggleDeleteAsync(productId, cancellationToken);
    
        // Construct a Redis key based on the product ID
        var redisKey = $"product_{productId}";

        // Get the product from Redis
        var productFromRedis = await _redisService.GetAsync<List<ProductViewModel>>(redisKey);
        if (productFromRedis == null || !productFromRedis.Any())
            return deletedProduct
                ? Result<string>.Success("The product has been deleted successfully!")
                : Result<string>.Success("The product has been undeleted successfully!");
        
        // Update the product's soft delete status in Redis
        var productToUpdate = productFromRedis.First();
        productToUpdate.IsDeleted = deletedProduct; 

        // Update the product in Redis
        await _redisService.SetAsync(redisKey, new List<ProductViewModel> { productToUpdate }, TimeSpan.FromDays(30));

        // Return a message based on the deletedProduct status
        return deletedProduct 
            ? Result<string>.Success("The product has been deleted successfully!") 
            : Result<string>.Success("The product has been undeleted successfully!");
    }
}