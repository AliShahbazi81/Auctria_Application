using AuctriaApplication.DataAccess.Entities.Stores;
using AuctriaApplication.Infrastructure.Services;
using AuctriaApplication.Infrastructure.Store.Services;
using AuctriaApplication.Infrastructure.Store.Services.Abstract;
using AuctriaApplication.Services.Membership.Services.Token;
using AuctriaApplication.Services.Membership.Services.Users;
using AuctriaApplication.Services.MessagingAPI.Services.Email;
using AuctriaApplication.Services.MessagingAPI.Services.Sms;
using AuctriaApplication.Services.Store.Services.Abstract;
using AuctriaApplication.Services.Validation.Services.Phone;
using Microsoft.AspNetCore.Identity;

namespace Auctria_Application.Extensions;

public static class OtherServicesExtension
{
    public static IServiceCollection AddOtherServices(this IServiceCollection services)
    {
        // ----------------------- Services -----------------------
        // Membership
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<UserService>();

        // Messaging
        // Email
        services.AddScoped<IEmailService, EmailService>();
        // SMS
        services.AddScoped<ISmsService, SmsService>();
        
        // Store
        services.AddScoped<IShoppingCartService, IShoppingCartService>();
        services.AddScoped<ICategoryService, ICategoryService>();
        services.AddScoped<IProductService, IProductService>();


        // ----------------------- Managers -----------------------
        // Membership
        services.AddScoped<IUserAccessor, UserAccessor>();
        
        // Store
        services.AddScoped<IShoppingCartManager, ShoppingCartManager>();
        services.AddScoped<ICategoryManager, CategoryManager>();
        services.AddScoped<IProductManager, ProductManager>();


        return services;
    }
}