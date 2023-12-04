using AuctriaApplication.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctriaApplication.DataAccess.Entities.Stores;

public class Payment : EntityBase
{
    public required string StripeChargeId { get; set; }
    public decimal Amount { get; set; }
    public CurrencyTypes CurrencyTypes { get; set; }
    public required PaymentStatus PaymentStatus { get; set; }
    public required string CustomerStripeId { get; set; }
    public string? PaymentMethodDetails { get; set; }
    public required string ReceiptUrl { get; set; }
    
    // Relationships
    public required Guid ShoppingCartId { get; set; }
    public virtual Cart Cart { get; set; }
}

public class PaymentBuilder : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasOne(x => x.Cart)
            .WithOne(x => x.Payment)
            .HasForeignKey<Payment>(x => x.ShoppingCartId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}