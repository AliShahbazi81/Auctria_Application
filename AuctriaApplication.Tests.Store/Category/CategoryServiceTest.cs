namespace AuctriaApplication.Tests.Store.Category;

public class Tests
{
    
    #region Setup
    
    private ApplicationDbContext _dbContext;
    private Mock<IDbContextFactory<ApplicationDbContext>> _mockDbContextFactory;
    private CategoryService _categoryService;

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

        _categoryService = new CategoryService(_mockDbContextFactory.Object);
    }
    
    #endregion
    
    #region Add
    
    [Test]
    public async Task AddAsync_WhenCategoryIsValid_ShouldReturnCategoryWithExpectedValues()
    {
        // Arrange
        var categoryDto = new CategoryDto
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid()
        };
        
        var creatorId = Guid.NewGuid();

        // Act
        var result = await _categoryService.AddAsync(creatorId, categoryDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(categoryDto.Name);
        result.Description.Should().Be(categoryDto.Description);
        result.CreatedAt.Should().NotBe(default);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(2));
    }
    
    [Test]
    public async Task AddAsync_WhenCategoryNameIsNotUnique_ShouldThrowException()
    {
        // Arrange
        var categoryDto = new CategoryDto
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid()
        };
        
        var creatorId = Guid.NewGuid();

        // Act
        await _categoryService.AddAsync(creatorId, categoryDto, CancellationToken.None);
        Func<Task> act = async () => await _categoryService.AddAsync(creatorId, categoryDto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
    
    [Test]
    public async Task AddAsync_WhenCategoryNameIsNotUnique_ShouldNotSaveCategory()
    {
        // Arrange
        var categoryDto = new CategoryDto
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid()
        };
        
        var creatorId = Guid.NewGuid();

        // Act
        await _categoryService.AddAsync(creatorId, categoryDto, CancellationToken.None);
        Func<Task> act = async () => await _categoryService.AddAsync(creatorId, categoryDto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
    
    #endregion
    
    #region Get
    
    [Test]
    public async Task GetByIdAsync_WhenCategoryExists_ShouldReturnCategoryWithExpectedValues()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryService.GetAsync(CancellationToken.None, newCategory.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(newCategory.Id);
        result.Name.Should().Be(newCategory.Name);
        result.Description.Should().Be(newCategory.Description);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(2));
    }
    
    [Test]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _categoryService.GetAsync(CancellationToken.None, Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>("Sorry, but we could not find the item you are looking for.");
        
    }
    
    #endregion
    
    #region GetList
    
    [Test]
    public async Task GetAllAsync_WhenCategoriesExist_ShouldReturnCategoriesWithExpectedValues()
    {
        // Arrange
        var newCategory1 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };
        
        var newCategory2 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };
        
        var newCategory3 = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };
        
        await _dbContext.Categories.AddRangeAsync(newCategory1, newCategory2, newCategory3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryService.GetListAsync(CancellationToken.None);

        // Assert
        var categoryViewModels = result.ToList();
        categoryViewModels.Should().NotBeNull();
        categoryViewModels.Should().HaveCount(3);
        categoryViewModels.Should().Contain(x => x.Id == newCategory1.Id);
        categoryViewModels.Should().Contain(x => x.Id == newCategory2.Id);
        categoryViewModels.Should().Contain(x => x.Id == newCategory3.Id);
    }
    
    [Test]
    public async Task GetAllAsync_WhenCategoriesDoNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        // Act
        var result = await _categoryService.GetListAsync(CancellationToken.None);

        // Assert
        var categoryViewModels = result.ToList();
        categoryViewModels.Should().NotBeNull();
        categoryViewModels.Should().HaveCount(0);
    }
    
    #endregion
    
    #region Update
    
    [Test]
    public async Task UpdateAsync_WhenCategoryExists_ShouldReturnCategoryWithExpectedValues()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        var updatedCategoryDto = new CategoryDto
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid()
        };

        // Act
        var result = await _categoryService.UpdateAsync(newCategory.Id, updatedCategoryDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(newCategory.Id);
        result.Name.Should().Be(updatedCategoryDto.Name);
        result.Description.Should().Be(updatedCategoryDto.Description);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(2));
    }
    
    [Test]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        var updatedCategoryDto = new CategoryDto
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid()
        };

        // Act
        Func<Task> act = async () => await _categoryService.UpdateAsync(Guid.NewGuid(), updatedCategoryDto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>("Sorry, but we could not find the item you are looking for.");
    }
    
    [Test]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ShouldNotSaveCategory()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        var updatedCategoryDto = new CategoryDto
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid()
        };

        // Act
        Func<Task> act = async () => await _categoryService.UpdateAsync(Guid.NewGuid(), updatedCategoryDto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>("Sorry, but we could not find the item you are looking for.");
    }
    
    #endregion
    
    #region Delete
    
    [Test]
    public async Task DeleteAsync_WhenCategoryExists_ShouldReturnTrue()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryService.DeleteAsync(newCategory.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task DeleteAsync_WhenCategoryDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _categoryService.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>("Sorry, but we could not find the item you are looking for.");
    }
    
    [Test]
    public async Task DeleteAsync_WhenCategoryDoesNotExist_ShouldNotSaveCategory()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            Description = "Test Description" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _categoryService.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>("Sorry, but we could not find the item you are looking for.");
    }
    
    #endregion
    
    #region IsCategory
    
    [Test]
    public async Task IsCategoryAsync_WhenCategoryExists_ShouldReturnTrue()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryService.IsCategoryAsync(newCategory.Id);

        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task IsCategoryAsync_WhenCategoryDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryService.IsCategoryAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }
    
    [Test]
    public async Task IsCategoryAsync_WhenCategoryWithNameExists_ShouldReturnTrue()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryService.IsCategoryAsync(categoryName: newCategory.Name);

        // Assert
        result.Should().BeTrue();
    }
    
    [Test]
    public async Task IsCategoryAsync_WhenCategoryWithNameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var newCategory = new DataAccess.Entities.Stores.Category
        {
            Name = "Category" + Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        };

        await _dbContext.Categories.AddAsync(newCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryService.IsCategoryAsync(categoryName: "Category" + Guid.NewGuid());

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