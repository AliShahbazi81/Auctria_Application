using AuctriaApplication.Domain.Results;
using AuctriaApplication.Services.Store.Dto.ViewModel;

namespace AuctriaApplication.Infrastructure.Store.Services.Abstract;

public interface IShoppingCartManager
{
    Task<Result<ShoppingCartViewModel>> AddProductToCartAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken);
}