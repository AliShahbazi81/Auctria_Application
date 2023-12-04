namespace AuctriaApplication.Services.Store.Dto.ViewModel;

public record ShoppingCartViewModel
{
    public required Guid Id { get; set; }
    public double Total { get; set; }
    public required string Currency { get; set; }
    public required string PaymentStatus { get; set; }
    public required DateTime CreatedAt{ get; set; }
    public List<ProductCartItemViewModel> Products { get; set; }
}