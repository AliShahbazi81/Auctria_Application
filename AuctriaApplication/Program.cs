using Auctria_Application.ExceptionHandling;
using Auctria_Application.ExceptionHandling.Authorization;
using Auctria_Application.Extensions;
using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.DataAccess.Entities.Users;
using AuctriaApplication.DataAccess.Seed;
using AuctriaApplication.Services.Redis.Services;
using AuctriaApplication.Services.Redis.Services.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -------------------------- Register Services --------------------------
builder.Services.AddDatabaseService(builder.Configuration, useSqlLite: true);
builder.Services.AddSwaggerService();
builder.Services.AddAuthenticationService();
builder.Services.AddIdentityService(builder.Configuration);
builder.Services.AddOtherServices(builder.Configuration);
builder.Services.AddSingleton<IRedisService, RedisService>();
// -------------------------- End of Register Services --------------------------

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -------------------------- Seeding Database --------------------------
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
var redisService = app.Services.GetRequiredService<IRedisService>(); 


try
{
    context.Database.Migrate();
    await DbInitializer.Initializer(context, userManager, roleManager, redisService);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
// -------------------------- End of Seeding Database --------------------------

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseCors("cors");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.Run();