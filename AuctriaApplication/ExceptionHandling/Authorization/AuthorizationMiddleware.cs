using Auctria_Application.Attribute;
using AuctriaApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Auctria_Application.ExceptionHandling.Authorization;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var action = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            var requiredPermissions = action.MethodInfo.GetCustomAttributes(typeof(RequiredPermissionAttribute), false)
                .Cast<RequiredPermissionAttribute>()
                .Select(a => a.Permission);

            var user = context.User;
            if (user.IsInRole(RoleTypes.SuperAdmin.ToString()))
            {
                await _next(context);
                return;
            }

            var userClaims = user.Claims.Where(c => c.Type == "Permission").Select(c => c.Value);
            if (requiredPermissions.Any(rp => userClaims.Contains(rp.ToString())))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access Denied");
                return;
            }
        }

        await _next(context);
    }
}