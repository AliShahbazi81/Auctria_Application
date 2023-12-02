using Auctria_Application.ExceptionHandling;
using Auctria_Application.ExceptionHandling.Authorization;
using Auctria_Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// -------------------------- Register Services --------------------------
builder.Services.AddDatabaseService(builder.Configuration, useSqlLite: true);
builder.Services.AddAuthenticationService();
builder.Services.AddIdentityService(builder.Configuration);
builder.Services.AddOtherServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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