using AuctriaApplication.Domain.Enums;

namespace AuctriaApplication.Services.ExchangeAPI.Services.Abstract;

public interface IExchangeService
{
    Task<decimal> ConvertCurrencyAsync(
        decimal total,
        CurrencyTypes targetCurrency);
}