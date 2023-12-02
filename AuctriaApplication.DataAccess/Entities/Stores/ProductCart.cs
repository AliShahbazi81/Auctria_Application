using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctriaApplication.DataAccess.Entities.Stores;

public class ProductCart
{
    public required Guid CartId { get; set; }
    public virtual Cart Cart { get; set; }

    public required Guid ProductId { get; set; }
    protected virtual Product Product { get; set; }
}

public class ProductCartBuilder : IEntityTypeConfiguration<ProductCart>
{
    public void Configure(EntityTypeBuilder<ProductCart> builder)
    {
        
    }
}