namespace Auctria_Application.Extensions;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy("cors", policy => policy
            .SetIsOriginAllowed(origin =>
                new Uri(origin).Host.Contains("localhost")
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

        return services;
    }
}