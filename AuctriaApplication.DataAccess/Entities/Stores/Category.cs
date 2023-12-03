using AuctriaApplication.DataAccess.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctriaApplication.DataAccess.Entities.Stores;

public class Category : EntityBase
{
    public required string Name { get; set; } = null!;
    public string? Description { get; set; }
    
    // Relationships
    public required Guid UserId { get; set; }
    public virtual User User{ get; set; }
    public virtual ICollection<Product> Products { get; set; } = null!;
}

public class CategoryBuilder : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}