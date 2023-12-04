using System.Text.Json;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Services.ExchangeAPI.Services.Abstract;
using AuctriaApplication.Services.ExchangeAPI.Services.Configuration;
using Microsoft.Extensions.Options;

namespace AuctriaApplication.Services.ExchangeAPI.Services;

public class ExchangeService : IExchangeService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey; 

    public ExchangeService(HttpClient httpClient, IOptions<ExchangeConfig> options)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey;
    }

    public async Task<decimal> ConvertCurrencyAsync(
        decimal total, 
        CurrencyTypes targetCurrency)
    {
        if (targetCurrency == CurrencyTypes.CAD)
        {
            return total; 
        }

        var conversionRate = await GetConversionRateAsync(CurrencyTypes.CAD.ToString(), targetCurrency.ToString());
        return total * conversionRate;
    }

    private async Task<decimal> GetConversionRateAsync(string fromCurrency, string toCurrency)
    {
        // Construct the API request URL
        var url = $"https://api.exchangerate-api.com/v4/latest/{fromCurrency}?apiKey={_apiKey}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error fetching currency conversion data.");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
        var rate = data.GetProperty("rates").GetProperty(toCurrency).GetDecimal();
        return rate;
    }
}