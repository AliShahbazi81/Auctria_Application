namespace AuctriaApplication.Services.Redis.Services.Abstract;

public interface IRedisService
{
    Task<T?> GetAsync<T>(string key) where T : class;

    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
}