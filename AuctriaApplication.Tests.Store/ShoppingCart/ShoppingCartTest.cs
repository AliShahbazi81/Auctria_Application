using AuctriaApplication.DataAccess.Entities.Stores;
using AuctriaApplication.Domain.Dto.ViewModel;
using AuctriaApplication.Domain.Enums;

namespace AuctriaApplication.Tests.Store.ShoppingCart;

public class Tests
{
    #region Setup
    
    private ApplicationDbContext _dbContext;
    private Mock<IDbContextFactory<ApplicationDbContext>> _mockDbContextFactory;
    private ShoppingCartService _shoppingCartService;

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

        _shoppingCartService = new ShoppingCartService(_mockDbContextFactory.Object);
    }
    
    #endregion

    #region Get
    
    [Test]
    public async Task Get_ShouldReturnShoppingCart()
    {
        var userId = Guid.NewGuid();
        // Arrange
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.GetAsync(userId, shoppingCart.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(shoppingCart.Id);
        result.UserId.Should().Be(userId);
        result.CreatedAt.Should().Be(shoppingCart.CreatedAt);
        result.Total.Should().Be(shoppingCart.Total);
        result.Currency.Should().Be(shoppingCart.Currency);
    }
    
    [Test]
    public async Task Get_ShouldReturnNull_WhenShoppingCartDoesNotExist()
    {
        var userId = Guid.NewGuid();
        // Arrange
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.GetAsync(userId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
    
    // Get Cost
    [Test]
    public async Task GetCost_ShouldReturnCost()
    {
        var userId = Guid.NewGuid();
        // Arrange
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.GetCostAsync(shoppingCart.Id);

        // Assert
        result.Should().Be(0);
    }
    
    // Get Cart for user
    [Test]
    public async Task GetCartForUser_ShouldReturnCart()
    {
        var userId = Guid.NewGuid();
        // Arrange
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            Payment = new Payment
            {
                Id = Guid.NewGuid(),
                PaymentStatus = PaymentStatus.Pending,
                Amount = 0,
                CreatedAt = DateTime.Now,
                StripeChargeId = "ChargeId",
                CustomerStripeId = "CustomerId",
                ReceiptUrl = "ReceiptUrl",
                ShoppingCartId = Guid.NewGuid(),
            }
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.GetCartForUserAsync(userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(shoppingCart.Id);
        result.UserId.Should().Be(userId);
        result.CreatedAt.Should().Be(shoppingCart.CreatedAt);
        result.Total.Should().Be(shoppingCart.Total);
        result.Currency.Should().Be(shoppingCart.Currency);
    }
    
    [Test]
    public async Task GetCartForUser_ShouldReturnNull_WhenCartDoesNotExist()
    {
        var userId = Guid.NewGuid();
        // Arrange
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.GetCartForUserAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion
    
    #region GetList
    
    [Test]
    public async Task GetList_ShouldReturnShoppingCartList()
    {
        var userId = Guid.NewGuid();
        // Arrange
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.GetListAsync(userId, CancellationToken.None);

        // Assert
        var shoppingCartViewModels = result as ShoppingCartViewModel[] ?? result.ToArray();
        shoppingCartViewModels.Should().NotBeNull();
        shoppingCartViewModels.Should().HaveCount(1);
        shoppingCartViewModels[0].Id.Should().Be(shoppingCart.Id);
        shoppingCartViewModels[0].Total.Should().Be((double)shoppingCart.Total);
        shoppingCartViewModels[0].Currency.Should().Be(shoppingCart.Currency.ToString());
    }
    
    [Test]
    public async Task GetList_ShouldReturnEmptyList_WhenShoppingCartDoesNotExist()
    {
        var userId = Guid.NewGuid();
        // Arrange
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.GetListAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        var shoppingCartViewModels = result as ShoppingCartViewModel[] ?? result.ToArray();
        shoppingCartViewModels.Should().NotBeNull();
        shoppingCartViewModels.Should().HaveCount(0);
    }
    
    [Test]
    public async Task GetList_ShouldReturnShoppingCartListWithProducts()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create category
        var category = new DataAccess.Entities.Stores.Category
        {
            Id = Guid.NewGuid(),
            Name = "Category" + Guid.NewGuid(),
            Description = "Description",
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Quantity = 1,
                    Product = new DataAccess.Entities.Stores.Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Product",
                        Description = "Description",
                        Price = 10,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "ImageUrl",
                        Quantity = 0,
                        UserId = userId,
                        CategoryId = category.Id,
                    }
                }
            }
        };

        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.GetListAsync(userId, CancellationToken.None);

        // Assert
        var shoppingCartViewModels = result as ShoppingCartViewModel[] ?? result.ToArray();
        shoppingCartViewModels.Should().NotBeNull();
        shoppingCartViewModels.Should().HaveCount(1);
        shoppingCartViewModels[0].Id.Should().Be(shoppingCart.Id);
        shoppingCartViewModels[0].Total.Should().Be((double)shoppingCart.Total);
        shoppingCartViewModels[0].Currency.Should().Be(shoppingCart.Currency.ToString());
        shoppingCartViewModels[0].Products.Should().NotBeNull();
        shoppingCartViewModels[0].Products.Should().HaveCount(1);
        shoppingCartViewModels[0].Products[0].Name.Should().Be(shoppingCart.ProductCarts.First().Product.Name);
    }
    
    [Test]
    public async Task GetList_ShouldReturnEmptyList_WhenShoppingCartDoesNotHaveProducts()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create category
        var category = new DataAccess.Entities.Stores.Category
        {
            Id = Guid.NewGuid(),
            Name = "Category" + Guid.NewGuid(),
            Description = "Description",
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>()
        };

        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.GetListAsync(userId, CancellationToken.None);

        // Assert
        var shoppingCartViewModels = result as ShoppingCartViewModel[] ?? result.ToArray();
        shoppingCartViewModels.Should().NotBeNull();
        shoppingCartViewModels.Should().HaveCount(1);
        shoppingCartViewModels[0].Id.Should().Be(shoppingCart.Id);
        shoppingCartViewModels[0].Total.Should().Be((double)shoppingCart.Total);
        shoppingCartViewModels[0].Currency.Should().Be(shoppingCart.Currency.ToString());
        shoppingCartViewModels[0].Products.Should().NotBeNull();
        shoppingCartViewModels[0].Products.Should().HaveCount(0);
    }
    
    

    #endregion
    
    #region AddOrUpdate
    
    [Test]
    public async Task AddOrUpdate_ShouldAddShoppingCart()
    {
        var userId = Guid.NewGuid();
        // Arrange
        
        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 0,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 0,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 0,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };
        
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();
        
        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.AddOrUpdateProductInCartAsync(
            shoppingCart.Id, 
            products[0].Id, 
            1,
            CancellationToken.None);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task AddOrUpdate_ShouldUpdateShoppingCart()
    {
        var userId = Guid.NewGuid();
        // Arrange
        
        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 0,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 0,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 0,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };
        
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();
        
        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                }
            }
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.AddOrUpdateProductInCartAsync(
            shoppingCart.Id, 
            products[0].Id, 
            2,
            CancellationToken.None);
        
        // Assert
        result.Should().BeTrue();
    }
    
    #endregion
    
    #region CreateCartForUser
    
    [Test]
    public async Task CreateCartForUser_ShouldCreateCart()
    {
        var userId = Guid.NewGuid();
        // Arrange
        
        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 0,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 0,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 0,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };
        
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.CreateCartForUserAsync(userId, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(2));
        result.Total.Should().Be(0);
        result.Currency.Should().Be(CurrencyTypes.CAD);
    }
    
    #endregion

    #region UpdateCartTotal

    [Test]
    public async Task UpdateCartTotal_ShouldUpdateCartTotal()
    {
        var userId = Guid.NewGuid();
        // Arrange
        
        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };
        
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[1].Id,
                    Quantity = 2,
                    Product = products[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[2].Id,
                    Quantity = 3,
                    Product = products[2]
                }
            }
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        await _shoppingCartService.UpdateCartTotalAsync(shoppingCart.Id, CancellationToken.None);
        
        // Assert
        shoppingCart.Total.Should().Be(140);
    }
    
    [Test]
    public async Task UpdateCartTotal_ShouldNotUpdateCartTotal_WhenCartIsNotFound()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[1].Id,
                    Quantity = 2,
                    Product = products[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[2].Id,
                    Quantity = 3,
                    Product = products[2]
                }
            }
        };

        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        await _shoppingCartService.UpdateCartTotalAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        shoppingCart.Total.Should().Be(0);
    }

    #endregion

    #region DeleteCart
    
    [Test]
    public async Task DeleteCart_ShouldDeleteCart()
    {
        var userId = Guid.NewGuid();
        // Arrange
        
        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };
        
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[1].Id,
                    Quantity = 2,
                    Product = products[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[2].Id,
                    Quantity = 3,
                    Product = products[2]
                }
            }
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.DeleteCartAsync(userId, shoppingCart.Id);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task DeleteCart_ShouldNotDeleteCart_WhenCartIsNotFound()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[1].Id,
                    Quantity = 2,
                    Product = products[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[2].Id,
                    Quantity = 3,
                    Product = products[2]
                }
            }
        };

        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.DeleteCartAsync(userId, Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    #endregion
    
    #region DeleteItemInCart

    [Test]
    public async Task DeleteItemInCart_ShouldDeleteItemInCart()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[1].Id,
                    Quantity = 2,
                    Product = products[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[2].Id,
                    Quantity = 3,
                    Product = products[2]
                }
            }
        };

        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.DeleteItemInCartAsync(userId, shoppingCart.Id, products[0].Id);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task DeleteItemInCart_ShouldNotDeleteItemInCart_WhenCartIsNotFound()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[1].Id,
                    Quantity = 2,
                    Product = products[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[2].Id,
                    Quantity = 3,
                    Product = products[2]
                }
            }
        };

        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.DeleteItemInCartAsync(userId, Guid.NewGuid(), products[0].Id);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task DeleteItemInCart_ShouldNotDeleteItemInCart_WhenProductIsNotFound()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product2",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product3",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = Guid.NewGuid(),
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[1].Id,
                    Quantity = 2,
                    Product = products[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[2].Id,
                    Quantity = 3,
                    Product = products[2]
                }
            }
        };

        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _shoppingCartService.DeleteItemInCartAsync(userId, shoppingCart.Id, Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    #endregion
    
    #region ToViewModel

    [Test]
    public async Task ToViewModel_ShouldReturnShoppingCartViewModel()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create category
        var category = new DataAccess.Entities.Stores.Category
        {
            Id = Guid.NewGuid(),
            Name = "Category" + Guid.NewGuid(),
            Description = "Description",
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product2",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product3",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = category.Id,
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[1].Id,
                    Quantity = 2,
                    Product = products[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = Guid.NewGuid(),
                    ProductId = products[2].Id,
                    Quantity = 3,
                    Product = products[2]
                }

            }
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = _shoppingCartService.ToViewModel(shoppingCart);
        
        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(shoppingCart.Id);
        result.CreatedAt.Should().BeCloseTo(shoppingCart.CreatedAt, TimeSpan.FromSeconds(5));
        result.Total.Should().Be((double)shoppingCart.Total);
        result.Currency.Should().Be(shoppingCart.Currency.ToString());
        result.Products.Should().NotBeNull();
        result.Products.Should().HaveCount(3);
        result.Products[0].Name.Should().Be(shoppingCart.ProductCarts.First().Product.Name);
        result.Products[1].Name.Should().Be(shoppingCart.ProductCarts.Skip(1).First().Product.Name);
        result.Products[2].Name.Should().Be(shoppingCart.ProductCarts.Skip(2).First().Product.Name);
    }

    #endregion
    
    #region AreItemsReduced

    [Test]
    public async Task AreItemsReduced_ShouldReturnTrue_WhenItemsAreReduced()
    {
        var userId = Guid.NewGuid();
        var shoppingCartId = Guid.NewGuid();
        // Arrange

        // Create category
        var category = new DataAccess.Entities.Stores.Category
        {
            Id = Guid.NewGuid(),
            Name = "Category" + Guid.NewGuid(),
            Description = "Description",
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product2",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product3",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = category.Id,
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            Id = shoppingCartId,
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = shoppingCartId,
                    ProductId = products[0].Id,
                    Quantity = 1,
                    Product = products[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = shoppingCartId,
                    ProductId = products[1].Id,
                    Quantity = 2,
                    Product = products[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CartId = shoppingCartId,
                    ProductId = products[2].Id,
                    Quantity = 3,

                }
            }
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _shoppingCartService.AreItemsReducedAsync(shoppingCartId);
        
        // Assert
        result.Item1.Should().BeTrue();
        result.Item2.Should().HaveCount(3);
    }


    #endregion

    #region IsShoppingCart
    
    [Test]
    public async Task IsShoppingCart_ShouldReturnTrue_WhenCartIsShoppingCart()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create category
        var category = new DataAccess.Entities.Stores.Category
        {
            Id = Guid.NewGuid(),
            Name = "Category" + Guid.NewGuid(),
            Description = "Description",
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product2",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product3",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = category.Id,
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>()
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _shoppingCartService.IsShoppingCartAsync(shoppingCart.Id);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task IsShoppingCart_ShouldReturnFalse_WhenCartIsNotShoppingCart()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create category
        var category = new DataAccess.Entities.Stores.Category
        {
            Id = Guid.NewGuid(),
            Name = "Category" + Guid.NewGuid(),
            Description = "Description",
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product2",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product3",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = category.Id,
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>()
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _shoppingCartService.IsShoppingCartAsync(Guid.NewGuid());
        
        // Assert
        result.Should().BeFalse();
    }

    #endregion
    
    #region IsCartPaid
    
    [Test]
    public async Task IsCartPaid_ShouldReturnTrue_WhenCartIsPaid()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create category
        var category = new DataAccess.Entities.Stores.Category
        {
            Id = Guid.NewGuid(),
            Name = "Category" + Guid.NewGuid(),
            Description = "Description",
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product2",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product3",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = category.Id,
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>(),
            Payment = new Payment
            {
                PaymentStatus = PaymentStatus.Succeeded,
                StripeChargeId = "StripeChargeId",
                CustomerStripeId = "CustomerStripeId",
                ReceiptUrl = "ReceiptUrl",
                ShoppingCartId = Guid.NewGuid(),
            }
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _shoppingCartService.IsCartPaidAsync(shoppingCart.Id);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task IsCartPaid_ShouldReturnFalse_WhenCartIsNotPaid()
    {
        var userId = Guid.NewGuid();
        // Arrange

        // Create category
        var category = new DataAccess.Entities.Stores.Category
        {
            Id = Guid.NewGuid(),
            Name = "Category" + Guid.NewGuid(),
            Description = "Description",
            CreatedAt = DateTime.Now,
            UserId = userId
        };

        // Create a list of products first
        var products = new List<DataAccess.Entities.Stores.Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Description",
                Price = 10,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 1,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product2",
                Description = "Description",
                Price = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 2,
                UserId = userId,
                CategoryId = category.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product3",
                Description = "Description",
                Price = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "ImageUrl",
                Quantity = 3,
                UserId = userId,
                CategoryId = category.Id,
            }
        };

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Create ShoppingCart
        var shoppingCart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.Now,
            Total = 0,
            Currency = CurrencyTypes.CAD,
            ProductCarts = new List<ProductCart>(),
            Payment = new Payment
            {
                PaymentStatus = PaymentStatus.Failed,
                StripeChargeId = "StripeChargeId",
                CustomerStripeId = "CustomerStripeId",
                ReceiptUrl = "ReceiptUrl",
                ShoppingCartId = Guid.NewGuid(),
            }
        };
        
        _dbContext.Carts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _shoppingCartService.IsCartPaidAsync(shoppingCart.Id);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Test]
    public async Task IsCartPaid_ShouldReturnFalse_WhenCartIsNotFound()
    {
        // Arrange

        // Act
        var result = await _shoppingCartService.IsCartPaidAsync(Guid.NewGuid());
        
        // Assert
        result.Should().BeNull();
    }
    
    #endregion
    
    
    [TearDown]
    public void CleanUp()
    {
        _dbContext.Dispose();
    }
}