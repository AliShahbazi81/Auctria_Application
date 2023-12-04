using AuctriaApplication.Domain.Dto.Base;

namespace AuctriaApplication.Domain.Dto.ViewModel;

public record CategoryViewModel : BaseCategoryDto
{
    public required Guid Id { get; set; }
    public required DateTime CreatedAt { get; set; }
}