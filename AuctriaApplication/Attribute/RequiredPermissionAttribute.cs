using AuctriaApplication.Domain.Enums;

namespace Auctria_Application.Attribute;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class RequiredPermissionAttribute
{
    public PermissionAction Permission { get; }

    public RequiredPermissionAttribute(PermissionAction permission)
    {
        Permission = permission;
    }
}