using System.ComponentModel;

namespace AuctriaApplication.Domain.Enums;

public enum PermissionAction
{
    #region MEMBERS
    [Description("See members list")]
    Members_List,
    
    [Description("Lockout a user")]
    Members_Lockout,
    
    #endregion
}