using AuctriaApplication.DataAccess.Entities.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctriaApplication.DataAccess.Entities.Users;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsDeleted { get; set; }
    public string DeletionReason { get; set; } = string.Empty;
    public DateTime SignedUpAt { get; set; } = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"));
    public DateTime? DeletionTime { get; set; }
    public Guid? DeletedBy { get; set; }


    // Relations
    public virtual ICollection<Category> Categories{ get; set; }
    public virtual ICollection<Product> Products{ get; set; }
    public virtual ICollection<Cart> ShoppingCarts { get; set; }
    public virtual UserTempValidation UserTempValidation { get; set; }
}

public class UserBuilder : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
    }
}