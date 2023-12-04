using AuctriaApplication.Services.Store.Dto.Base;

namespace AuctriaApplication.Services.Store.Dto;

public record ProductDto : BaseProductDto
{
    public required double Price { get; set; }
}