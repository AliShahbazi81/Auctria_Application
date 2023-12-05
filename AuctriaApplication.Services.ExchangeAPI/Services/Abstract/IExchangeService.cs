using AuctriaApplication.Domain.Enums;

namespace AuctriaApplication.Services.ExchangeAPI.Services.Abstract;

public interface IExchangeService
{
    /// <summary>
    /// Gets the conversion rate from the base currency to the specified currency.
    /// </summary>
    /// <param name="toCurrency">Set the target of the currency to be converted</param>
    /// <returns></returns>
    Task<double> GetConversionRateAsync(string toCurrency);
}