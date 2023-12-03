using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctriaApplication.DataAccess.Entities.Stores;

public class ProductCart
{
    public Guid Id { get; set; }
    public required int Quantity { get; set; }
    public required Guid CartId { get; set; }
    public virtual Cart Cart { get; set; }

    public required Guid ProductId { get; set; }
    public virtual Product Product { get; set; }
}

public class ProductCartBuilder : IEntityTypeConfiguration<ProductCart>
{
    public void Configure(EntityTypeBuilder<ProductCart> builder)
    {
        builder.HasOne(x => x.Cart)
            .WithMany(x => x.ProductCarts)
            .HasForeignKey(x => x.CartId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.ProductCarts)
            .HasForeignKey(x => x.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}