namespace AuctriaApplication.Services.Redis.Services.Abstract;

public interface IRedisService
{
    /// <summary>
    /// Gets a value from the cache.
    /// </summary>
    /// <param name="key"> Key of the dictionary </param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    ///  Sets a value in the cache.
    /// </summary>
    /// <param name="key">Sets the key value of the dictionary</param>
    /// <param name="value">Sets the value of the dictionary</param>
    /// <param name="expiry">Sets the expiry time of the record</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
}