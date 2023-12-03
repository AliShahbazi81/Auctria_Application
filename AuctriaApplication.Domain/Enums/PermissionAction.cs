using System.ComponentModel;

namespace AuctriaApplication.Domain.Enums;

public enum PermissionAction
{
    #region PRODUCT
    [Description("Add a product")]
    Product_Create,
    
    [Description("Update a product")]
    Product_Update,
    
    [Description("Delete a product")]
    Porduct_Delete,
    
    #endregion

    #region CATEGORY

    [Description("Create a category")]
    Category_Create,
    
    [Description("Update a category")]
    Category_Update,
    
    [Description("Delete a category")]
    Category_Delete,

    #endregion
    
    #region MEMBERS
    [Description("See members list")]
    Members_List,
    
    [Description("Lockout a user")]
    Members_Lockout,
    
    #endregion
}