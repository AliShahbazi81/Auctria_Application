using AuctriaApplication.Infrastructure.Results;
using AuctriaApplication.Services.Payment.Dto;

namespace AuctriaApplication.Infrastructure.Payment.Services.Abstract;

public interface IPaymentManager
{
    Task<Result<string>> PayAsync(
        Guid shoppingCartId,
        UserCardInfoDto cardDto);
}