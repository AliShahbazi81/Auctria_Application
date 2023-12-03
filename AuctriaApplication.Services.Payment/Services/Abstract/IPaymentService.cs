using AuctriaApplication.Services.Payment.Dto;

namespace AuctriaApplication.Services.Payment.Services.Abstract;

/// <summary>
/// Provides services for processing payments.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Asynchronously processes a payment for a shopping cart.
    /// </summary>
    /// <param name="shoppingCardId">The unique identifier of the shopping cart.</param>
    /// <param name="cardDto">The payment card information.</param>
    /// <param name="cost">The total cost of the payment.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the payment was successful.</returns>
    Task<bool> PayAsync(
        Guid shoppingCardId,
        UserCardInfoDto cardDto,
        decimal cost);

    /// <summary>
    /// Asynchronously checks if a shopping cart has been paid for.
    /// </summary>
    /// <param name="shoppingCartId">The unique identifier of the shopping cart.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the shopping cart has been paid for.</returns>
    Task<bool> IsPaidAsync(Guid shoppingCartId);
}