using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.Services.Redis.Services;
using AuctriaApplication.Services.Redis.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Auctria_Application.Extensions;

public static class DatabaseServiceExtension
{
    public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration config, bool useSqlLite = false)
    {
        if (useSqlLite)
        {
            // Configure the DbContext to use SQLite
            services.AddDbContextFactory<ApplicationDbContext>(opt =>
                opt.UseSqlite(config.GetConnectionString("SqliteConnection")));
        }
        else
        {
            // Configure the DbContext to use SQL Server (or any other database)
            services.AddDbContextFactory<ApplicationDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        }
        
        // Configure and register Redis
        var redisConnectionString = config.GetSection("Redis:ConnectionString").Value;
        services.AddSingleton<IRedisService>(sp => 
            new RedisService(redisConnectionString!, sp.GetRequiredService<ILogger<RedisService>>()));
        
        return services;
    }
}