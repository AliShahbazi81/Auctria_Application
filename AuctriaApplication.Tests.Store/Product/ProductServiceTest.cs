namespace AuctriaApplication.Tests.Store.Product;

public class Tests
{
    #region Setup
    
    private ApplicationDbContext _dbContext;
    private Mock<IDbContextFactory<ApplicationDbContext>> _mockDbContextFactory;
    private ProductService _productService;

    [SetUp]
    public void Setup()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _dbContext = new ApplicationDbContext(options);

        // Mock IDbContextFactory
        _mockDbContextFactory = new Mock<IDbContextFactory<ApplicationDbContext>>();
        _mockDbContextFactory.Setup(factory => factory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_dbContext);

        _productService = new ProductService(_mockDbContextFactory.Object);
    }
    
    #endregion
   
    #region Get
    
    [Test]
    public async Task GetAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetAsync(CancellationToken.None, newProduct.Id);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(1);
        productViewModels[0].Name.Should().Be(newProduct.Name);
        productViewModels[0].Price.Should().Be((double)newProduct.Price);
        productViewModels[0].Description.Should().Be(newProduct.Description);
        productViewModels[0].CategoryId.Should().Be(newProduct.CategoryId);
        productViewModels[0].Quantity.Should().Be(newProduct.Quantity);
    }
    
    [Test]
    public async Task GetAsync_WhenProductDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetAsync(CancellationToken.None, Guid.NewGuid());
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    [Test]
    public async Task GetAsync_WhenProductIsDeleted_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetAsync(CancellationToken.None, newProduct.Id);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    [Test]
    public async Task GetAsync_WhenProductIsNotDeleted_ReturnsProduct()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetAsync(CancellationToken.None, newProduct.Id);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(1);
        productViewModels[0].Name.Should().Be(newProduct.Name);
        productViewModels[0].Price.Should().Be((double)newProduct.Price);
        productViewModels[0].Description.Should().Be(newProduct.Description);
        productViewModels[0].CategoryId.Should().Be(newProduct.CategoryId);
        productViewModels[0].Quantity.Should().Be(newProduct.Quantity);
    }
    
    // Get product by name
    [Test]
    public async Task GetAsync_WhenProductExists_ReturnsProductByName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetAsync(CancellationToken.None, productName: newProduct.Name);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(1);
        productViewModels[0].Name.Should().Be(newProduct.Name);
        productViewModels[0].Price.Should().Be((double)newProduct.Price);
        productViewModels[0].Description.Should().Be(newProduct.Description);
        productViewModels[0].CategoryId.Should().Be(newProduct.CategoryId);
        productViewModels[0].Quantity.Should().Be(newProduct.Quantity);
    }
    
    [Test]
    public async Task GetAsync_WhenProductDoesNotExist_ReturnsEmptyListByName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetAsync(CancellationToken.None, productName: "Product" + Guid.NewGuid());
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    // GetProductById
    [Test]
    public async Task GetProductByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetProductByIdAsync(newProduct.Id);
        
        // Assert
        result.Name.Should().Be(newProduct.Name);
        result.Price.Should().Be(newProduct.Price);
        result.Description.Should().Be(newProduct.Description);
        result.CategoryId.Should().Be(newProduct.CategoryId);
        result.Quantity.Should().Be(newProduct.Quantity);
    }
    
    [Test]
    public void GetProductByIdAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        
        // Act
        Func<Task> act = async () => await _productService.GetProductByIdAsync(productId);
        
        // Assert
        act.Should().ThrowAsync<NotFoundException>();
    }
    
    
    #endregion

    #region GetList

    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProducts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(2);
        productViewModels[0].Name.Should().Be(newProduct1.Name);
        productViewModels[0].Price.Should().Be((double)newProduct1.Price);
        productViewModels[0].Description.Should().Be(newProduct1.Description);
        productViewModels[0].CategoryId.Should().Be(newProduct1.CategoryId);
        productViewModels[0].Quantity.Should().Be(newProduct1.Quantity);
        productViewModels[1].Name.Should().Be(newProduct2.Name);
        productViewModels[1].Price.Should().Be((double)newProduct2.Price);
        productViewModels[1].Description.Should().Be(newProduct2.Description);
        productViewModels[1].CategoryId.Should().Be(newProduct2.CategoryId);
        productViewModels[1].Quantity.Should().Be(newProduct2.Quantity);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByCategoryName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, categoryName: newCategory1.Name);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(1);
        productViewModels[0].Name.Should().Be(newProduct1.Name);
        productViewModels[0].Price.Should().Be((double)newProduct1.Price);
        productViewModels[0].Description.Should().Be(newProduct1.Description);
        productViewModels[0].CategoryId.Should().Be(newProduct1.CategoryId);
        productViewModels[0].Quantity.Should().Be(newProduct1.Quantity);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByCategoryNameAndProductName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, categoryName: newCategory1.Name, productName: newProduct1.Name);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(1);
        productViewModels[0].Name.Should().Be(newProduct1.Name);
        productViewModels[0].Price.Should().Be((double)newProduct1.Price);
        productViewModels[0].Description.Should().Be(newProduct1.Description);
        productViewModels[0].CategoryId.Should().Be(newProduct1.CategoryId);
        productViewModels[0].Quantity.Should().Be(newProduct1.Quantity);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByCategoryNameAndProductNameAndMinPrice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 5.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 15.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, categoryName: newCategory1.Name, productName: newProduct1.Name, minPrice: 10.00);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByCategoryNameAndProductNameAndMaxPrice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 5.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 15.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, categoryName: newCategory1.Name, productName: newProduct1.Name, maxPrice: 10.00);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(1);
        productViewModels[0].Name.Should().Be(newProduct1.Name);
        productViewModels[0].Price.Should().Be((double)newProduct1.Price);
        productViewModels[0].Description.Should().Be(newProduct1.Description);
        productViewModels[0].CategoryId.Should().Be(newProduct1.CategoryId);
        productViewModels[0].Quantity.Should().Be(newProduct1.Quantity);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByCategoryNameAndProductNameAndMinPriceAndMaxPrice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 5.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 15.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, categoryName: newCategory1.Name, productName: newProduct1.Name, minPrice: 10.00, maxPrice: 10.00);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByCategoryNameAndMinPrice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 5.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 15.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, categoryName: newCategory1.Name, minPrice: 10.00);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByCategoryNameAndMaxPrice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 5.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 15.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, categoryName: newCategory1.Name, maxPrice: 10.00);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByCategoryNameAndMinPriceAndMaxPrice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 5.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 15.00m,
            Description = "Description" + Guid.NewGuid(),

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, categoryName: newCategory1.Name, minPrice: 10.00, maxPrice: 10.00);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByProductNameAndMinPriceAndMaxPrice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name" + Guid.NewGuid(),
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();

        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 5.00m,
            Description = "Description",

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 15.00m,
            Description = "Description",

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, productName: newProduct1.Name, minPrice: 10.00, maxPrice: 10.00);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    [Test]
    public async Task GetListAsync_WhenProductsExist_ReturnsProductsByProductNameAndMinPrice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name",
            UserId = userId
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category Name",
            UserId = userId
        };
        
        await _dbContext.Categories.AddAsync(newCategory1);
        await _dbContext.Categories.AddAsync(newCategory2);
        await _dbContext.SaveChangesAsync();
        
        var newProduct1 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 5.00m,
            Description = "Description",

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory1.Id,
        };
        
        var newProduct2 = new DataAccess.Entities.Stores.Product
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 15.00m,
            Description = "Description",

            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            IsDeleted = true,
            UserId = userId,
            CategoryId = newCategory2.Id,
        };
        
        await _dbContext.Products.AddAsync(newProduct1);
        await _dbContext.Products.AddAsync(newProduct2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _productService.GetListAsync(CancellationToken.None, productName: newProduct1.Name, minPrice: 10.00);
        
        // Assert
        var productViewModels = result.ToList();
        productViewModels.Should().HaveCount(0);
    }
    
    

    #endregion
    
    #region Add
    
    [Test]
    public async Task AddAsync_WhenCategoryExists_ReturnsProduct()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            UserId = userId,
            Name = "Category Name" + Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var productDto = new ProductDto
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00,
            Description = "Description" + Guid.NewGuid(),
            Quantity = 0,
            ImageUrl = "https://www.google.com"
        };
        
        // Act
        var result = await _productService.AddAsync(userId, newCategory.Id, productDto, CancellationToken.None);
        
        // Assert
        result.Name.Should().Be(productDto.Name);
        result.Price.Should().Be(productDto.Price);
        result.Description.Should().Be(productDto.Description);
        result.CategoryId.Should().Be(newCategory.Id);
        result.Quantity.Should().Be(productDto.Quantity);
    }
    
    [Test]
    public async Task AddAsync_WhenCategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var productDto = new ProductDto
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00,
            Description = "Description" + Guid.NewGuid(),
            Quantity = 0,
            ImageUrl = "https://www.google.com"
        };
        
        // Act
        Func<Task> act = async () => await _productService.AddAsync(userId, Guid.NewGuid(), productDto, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    
    #endregion
    
    #region Update
    
    [Test]
    public async Task UpdateAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            UserId = userId,
            Name = "Category Name" + Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            UserId = userId,
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CategoryId = newCategory.Id
        };

        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        var productDto = new ProductDto
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00,
            Description = "Description" + Guid.NewGuid(),
            Quantity = 0,
            ImageUrl = "https://www.google.com"
        };
        
        // Act
        var result = await _productService.UpdateAsync(newProduct.Id, productDto, CancellationToken.None);
        
        // Assert
        result.Name.Should().Be(productDto.Name);
        result.Price.Should().Be(productDto.Price);
        result.Description.Should().Be(productDto.Description);
        result.CategoryId.Should().Be(newCategory.Id);
        result.Quantity.Should().Be(productDto.Quantity);
    }
    
    [Test]
    public async Task UpdateAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var productDto = new ProductDto
        {
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00,
            Description = "Description" + Guid.NewGuid(),
            Quantity = 0,
            ImageUrl = "https://www.google.com"
        };
        
        // Act
        Func<Task> act = async () => await _productService.UpdateAsync(Guid.NewGuid(), productDto, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    
    
    #endregion
    
    #region ToggleDelete
    
    [Test]
    public async Task ToggleDeleteAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            UserId = userId,
            Name = "Category Name" + Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            UserId = userId,
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CategoryId = newCategory.Id
        };

        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.ToggleDeleteAsync(newProduct.Id, CancellationToken.None);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task ToggleDeleteAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        
        // Act
        Func<Task> act = async () => await _productService.ToggleDeleteAsync(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    #endregion
    
    #region IsProduct
    
    [Test]
    public async Task IsProductAsync_WhenProductExists_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            UserId = userId,
            Name = "Category Name" + Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            UserId = userId,
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CategoryId = newCategory.Id
        };

        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.IsProductAsync(newProduct.Id);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task IsProductAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        
        // Act
        var result = await _productService.IsProductAsync(Guid.NewGuid());
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Test]
    public async Task IsProductAsync_WhenProductExists_ReturnsTrueByName()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            UserId = userId,
            Name = "Category Name" + Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();
        
        var newProduct = new DataAccess.Entities.Stores.Product
        {
            UserId = userId,
            Name = "Product" + Guid.NewGuid(),
            Price = 10.00m,
            Description = "Description" + Guid.NewGuid(),
            Quantity = 0,
            ImageUrl = "https://www.google.com",
            CategoryId = newCategory.Id
        };

        await _dbContext.Products.AddAsync(newProduct);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _productService.IsProductAsync(productName: newProduct.Name);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task IsProductAsync_WhenProductDoesNotExist_ReturnsFalseByName()
    {
        // Arrange
        
        // Act
        var result = await _productService.IsProductAsync(productName: "Product" + Guid.NewGuid());
        
        // Assert
        result.Should().BeFalse();
    }
    
    #endregion
    
    [TearDown]
    public void CleanUp()
    {
        _dbContext.Dispose();
    }
}