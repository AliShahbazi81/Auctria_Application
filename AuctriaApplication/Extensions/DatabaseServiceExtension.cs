using AuctriaApplication.DataAccess.DbContext;
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

        return services;
    }
}