namespace AuctriaApplication.Services.Redis.Services.Configuration;

public record  RedisConfig
{
    public string ConnectionString { get; set; }
}