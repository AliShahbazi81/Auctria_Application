using AuctriaApplication.Domain.Dto.Base;

namespace AuctriaApplication.Domain.Dto;

public record ProductDto : BaseProductDto
{
    public required double Price { get; set; }
}