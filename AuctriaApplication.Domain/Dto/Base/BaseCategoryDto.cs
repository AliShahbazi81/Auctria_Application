namespace AuctriaApplication.Domain.Dto.Base;

public record BaseCategoryDto 
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}