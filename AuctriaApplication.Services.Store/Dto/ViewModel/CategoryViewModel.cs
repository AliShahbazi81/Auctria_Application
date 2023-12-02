using AuctriaApplication.Services.Store.Dto.Base;

namespace AuctriaApplication.Services.Store.Dto.ViewModel;

public record CategoryViewModel : BaseCategoryDto
{
    public required Guid Id { get; set; }
    public required DateTime CreatedAt { get; set; }
}