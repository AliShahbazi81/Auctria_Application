using AuctriaApplication.Services.Store.Dto.Base;

namespace AuctriaApplication.Services.Store.Dto.ViewModel;

public record  ProductViewModel : BaseProductDto
{
    public required Guid Id { get; set; }
    public required string CategoryName { get; set; }
    public required double Price { get; set; }
    public string? Currency { get; set; } = null;
    public bool IsDeleted { get; set; }
}