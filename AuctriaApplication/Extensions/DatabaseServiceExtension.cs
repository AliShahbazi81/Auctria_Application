using AuctriaApplication.DataAccess.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Auctria_Application.Extensions;

public static class DatabaseServiceExtension
{
    public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContextFactory<ApplicationDbContext>(opt => 
            { opt.UseSqlServer(config.GetConnectionString("DefaultConnection")); });
        
        return services;
    }
}