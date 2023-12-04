namespace AuctriaApplication.Domain.Dto.ViewModel;

public record ProductCartItemViewModel
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; }
}