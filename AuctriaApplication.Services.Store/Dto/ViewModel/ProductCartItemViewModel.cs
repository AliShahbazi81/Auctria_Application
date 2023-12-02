namespace AuctriaApplication.Services.Store.Dto.ViewModel;

public record struct ProductCartItemViewModel
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; }
}