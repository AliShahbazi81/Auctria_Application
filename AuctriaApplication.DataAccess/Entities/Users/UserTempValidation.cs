using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctriaApplication.DataAccess.Entities.Users;

public sealed class UserTempValidation
{
    public Guid Id { get; set; }
    public int TypeValidation { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime Expiry { get; set; } = DateTime.UtcNow.AddMinutes(5);

    // Relations
    public User User{ get; set; }
    public Guid UserId { get; set; }
}

public class UserTempValidationBuilder : IEntityTypeConfiguration<UserTempValidation>
{
    public void Configure(EntityTypeBuilder<UserTempValidation> builder)
    {
        builder.ToTable("UserTempValidation");

        builder.HasOne(x => x.User)
            .WithOne(x => x.UserTempValidation)
            .HasForeignKey<UserTempValidation>(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}