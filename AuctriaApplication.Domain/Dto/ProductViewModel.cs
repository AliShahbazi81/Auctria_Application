using AuctriaApplication.Domain.Dto.Base;

namespace AuctriaApplication.Domain.Dto;

public record  ProductViewModel : BaseProductDto
{
    public required Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public required double Price { get; set; }
    public string? Currency { get; set; } = null;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}