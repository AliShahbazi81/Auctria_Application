using AuctriaApplication.DataAccess.Entities.Users;
using AuctriaApplication.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctriaApplication.DataAccess.Entities.Stores;

public class Cart : EntityBase
{
    public required decimal Total { get; set; }
    
    // Relationships
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual Payment Payment { get; set; }
    
    public virtual ICollection<ProductCart> ProductCarts { get; set; } = null!;
}

public class CartBuilder : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasOne(x => x.User)
            .WithMany(x => x.ShoppingCarts)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}