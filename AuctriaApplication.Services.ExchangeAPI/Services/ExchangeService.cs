using System.Text.Json;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Services.ExchangeAPI.Services.Abstract;
using AuctriaApplication.Services.ExchangeAPI.Services.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AuctriaApplication.Services.ExchangeAPI.Services;

public class ExchangeService : IExchangeService
{
    private readonly MemoryCache _cache = new (new MemoryCacheOptions());
    private readonly HttpClient _httpClient;
    private readonly string _apiKey; 

    public ExchangeService(HttpClient httpClient, IOptions<ExchangeConfig> options)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey;
    }

    public async Task<double> GetConversionRateAsync(string toCurrency)
    {
        var cacheKey = $"{CurrencyTypes.CAD.ToString()}_{toCurrency}";

        // Check if the rate is in cache
        if (_cache.TryGetValue(cacheKey, out double rate))
        {
            return rate;
        }

        // Construct the API request URL
        var url = $"https://api.exchangerate-api.com/v4/latest/{CurrencyTypes.CAD.ToString()}?apiKey={_apiKey}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error fetching currency conversion data.");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
        rate = data.GetProperty("rates").GetProperty(toCurrency).GetDouble();

        // Cache the rate for future use
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1)); 
        _cache.Set(cacheKey, rate, cacheEntryOptions);

        return Math.Round(rate, 2);
    }
}