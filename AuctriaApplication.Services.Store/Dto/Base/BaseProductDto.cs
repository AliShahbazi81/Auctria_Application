namespace AuctriaApplication.Services.Store.Dto.Base;

public record BaseProductDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
}