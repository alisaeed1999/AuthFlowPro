namespace AuthFlowPro.Application.Permission;

public static class RolePermissions
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Basic = "Basic";

    public static List<string> All = new()
    {
        Admin,
        Manager,
        Basic
    };

    public static Dictionary<string, List<string>> PermissionsByRole = new()
    {
        [Admin] = new List<string>
        {
            Permissions.User.View,
            Permissions.User.Create,
            Permissions.User.Edit,
            Permissions.User.Delete,
            Permissions.Role.View,
            Permissions.Role.Create,
            Permissions.Role.Edit,
            Permissions.Role.Delete,
            Permissions.Product.View,
            Permissions.Product.Create,
            Permissions.Product.Edit,
            Permissions.Product.Delete,
        },
        [Manager] = new List<string>
        {
            Permissions.User.View,
            Permissions.Product.View,
            Permissions.Product.Create,
        },
        [Basic] = new List<string>
        {
            Permissions.Product.View
        }
    };
}
