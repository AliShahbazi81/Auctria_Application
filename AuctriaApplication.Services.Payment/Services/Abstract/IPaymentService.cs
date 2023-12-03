using AuctriaApplication.Services.Payment.Dto;

namespace AuctriaApplication.Services.Payment.Services.Abstract;

public interface IPaymentService
{
    Task<bool> PayAsync(
        Guid shoppingCardId,
        UserCardInfoDto cardDto,
        decimal cost);

    Task<bool> IsPaidAsync(Guid shoppingCartId);
}