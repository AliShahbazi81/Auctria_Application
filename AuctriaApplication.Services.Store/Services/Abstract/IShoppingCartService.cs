﻿using AuctriaApplication.DataAccess.Entities.Stores;
using AuctriaApplication.Services.Store.Dto.ViewModel;

namespace AuctriaApplication.Services.Store.Services.Abstract;

public interface IShoppingCartService
{

    Task<ShoppingCartViewModel> GetAsync(
        Guid userId,
        Guid cartId,
        CancellationToken cancellationToken);

    Task<IEnumerable<ShoppingCartViewModel>> GetListAsync(
        Guid userId,
        CancellationToken cancellationToken);
    
    
    Task<bool> AddOrUpdateProductInCartAsync(
        Guid cartId, 
        Guid productId, 
        int quantity, 
        CancellationToken cancellationToken);

    Task<Cart?> GetCartForUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Cart> CreateCartForUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task UpdateCartTotalAsync(
        Guid cartId,
        CancellationToken cancellationToken);

    ShoppingCartViewModel ToViewModel(Cart cart);
    
}