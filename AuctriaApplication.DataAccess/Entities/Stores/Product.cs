using AuctriaApplication.DataAccess.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctriaApplication.DataAccess.Entities.Stores;

public class Product : EntityBase
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public required int Quantity { get; set; }
    public required decimal Price { get; set; }
    public bool IsDeleted { get; set; }
    
    // Relationships
    public required Guid AddedBy { get; set; }
    public virtual User User{ get; set; }
    public required Guid CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<ProductCart> ProductCarts { get; set; } = null!;
}

public class ProductBuilder : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasOne(x => x.User)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.AddedBy)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}