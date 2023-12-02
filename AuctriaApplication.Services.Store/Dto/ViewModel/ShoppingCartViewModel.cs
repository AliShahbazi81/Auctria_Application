namespace AuctriaApplication.Services.Store.Dto.ViewModel;

public record struct ShoppingCartViewModel
{
    public required Guid Id { get; set; }
    public required string Total { get; set; }
    public required string PaymentStatus { get; set; }
    public required DateTime CreatedAt{ get; set; }
    public List<ProductCartItemViewModel> Products { get; set; }
}