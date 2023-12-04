using AuctriaApplication.Infrastructure.Membership.Services;
using AuctriaApplication.Infrastructure.Membership.Services.Abstract;
using AuctriaApplication.Infrastructure.Payment.Services;
using AuctriaApplication.Infrastructure.Payment.Services.Abstract;
using AuctriaApplication.Infrastructure.Services;
using AuctriaApplication.Infrastructure.Services.Abstract;
using AuctriaApplication.Infrastructure.Store.Services;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using AuctriaApplication.Services.ExchangeAPI.Services;
using AuctriaApplication.Services.ExchangeAPI.Services.Abstract;
using AuctriaApplication.Services.ExchangeAPI.Services.Configuration;
using AuctriaApplication.Services.Membership.Services.Token;
using AuctriaApplication.Services.Membership.Services.Users;
using AuctriaApplication.Services.Membership.Services.Users.Abstract;
using AuctriaApplication.Services.MessagingAPI.Services.Email;
using AuctriaApplication.Services.MessagingAPI.Services.Sms;
using AuctriaApplication.Services.MessagingAPI.Services.Sms.Configuration;
using AuctriaApplication.Services.Payment.Services;
using AuctriaApplication.Services.Payment.Services.Abstract;
using AuctriaApplication.Services.Payment.Services.Configuration;
using AuctriaApplication.Services.Store.Services;
using AuctriaApplication.Services.Store.Services.Abstract;
using Microsoft.Extensions.Options;

namespace Auctria_Application.Extensions;

public static class OtherServicesExtension
{
    public static IServiceCollection AddOtherServices(this IServiceCollection services, IConfiguration config)
    {
        // ----------------------- Services -----------------------
        // Membership
        services.AddScoped<ITokenService, TokenService>();
        services.Configure<TokenConfig>(config.GetSection("JWTSettings"));
        services.AddSingleton(x => x.GetRequiredService<IOptions<TokenConfig>>().Value);
        services.AddScoped<IUserService, UserService>();

        // Messaging
        // Email
        services.AddScoped<IEmailService, EmailService>();
        // SMS
        services.AddScoped<ISmsService, SmsService>();
        services.Configure<SmsConfiguration>(config.GetSection("Twilio"));
        services.AddSingleton(x => x.GetRequiredService<IOptions<SmsConfiguration>>().Value);
        
        //Payment
        services.Configure<PaymentConfig>(config.GetSection("Stripe"));
        services.AddSingleton(x => x.GetRequiredService<IOptions<PaymentConfig>>().Value);
        services.AddScoped<IPaymentService, PaymentService>();
        
        // Store
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        
        // Exchange
        services.Configure<ExchangeConfig>(config.GetSection("Exchange"));
        services.AddSingleton(x => x.GetRequiredService<IOptions<ExchangeConfig>>().Value);
        services.AddScoped<IExchangeService, ExchangeService>();


        // ----------------------- Managers -----------------------
        // Membership
        services.AddScoped<IUserAccessor, UserAccessor>();
        services.AddScoped<IUserManager, UserManager>();
        
        // Payment
        services.AddScoped<IPaymentManager, PaymentManager>();
        
        // Store
        services.AddScoped<IShoppingCartManager, ShoppingCartManager>();
        services.AddScoped<ICategoryManager, CategoryManager>();
        services.AddScoped<IProductManager, ProductManager>();
        
        return services;
    }
}