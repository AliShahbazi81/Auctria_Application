using AuctriaApplication.Domain.Enums;

namespace AuctriaApplication.Services.ExchangeAPI.Services.Abstract;

public interface IExchangeService
{
    Task<double> GetConversionRateAsync(string toCurrency);
}