using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Services.Payment.Dto;

namespace AuctriaApplication.Infrastructure.Payment.Services.Abstract;

/// <summary>
/// Manages payment-related operations.
/// </summary>
public interface IPaymentManager
{
    /// <summary>
    /// Processes the payment for a given shopping cart.
    /// </summary>
    /// <param name="shoppingCartId">The unique identifier of the shopping cart.</param>
    /// <param name="cardDto">The payment card information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string message indicating the success or failure of the payment.</returns>
    Task<Result<string>> PayAsync(
        Guid shoppingCartId,
        UserCardInfoDto cardDto);
}