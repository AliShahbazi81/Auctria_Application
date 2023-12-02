using System.ComponentModel.DataAnnotations;

namespace AuctriaApplication.DataAccess.Entities;

public class EntityBase
{
    [Key]
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
}