using System.Reflection;

namespace AuthFlowPro.Application.Permission;

public static class Permissions
{
    public static class User
    {
        public const string View = "Permissions.User.View";
        public const string Create = "Permissions.User.Create";
        public const string Edit = "Permissions.User.Edit";
        public const string Delete = "Permissions.User.Delete";
    }

    public static class Role
    {
        public const string View = "Permissions.Role.View";
        public const string Create = "Permissions.Role.Create";
        public const string Edit = "Permissions.Role.Edit";
        public const string Delete = "Permissions.Role.Delete";
    }

    public static class Product
    {
        public const string View = "Permissions.Product.View";
        public const string Create = "Permissions.Product.Create";
        public const string Edit = "Permissions.Product.Edit";
        public const string Delete = "Permissions.Product.Delete";
    }

    public static class PermissionHelper
    {
        public static List<string> GetAllPermissions()
        {
            return GetPermissionsByModule(); // returns all if no filter
        }

        public static List<string> GetPermissionsByModule(string? module = null)
        {
            var permissionList = new List<string>();

            var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var type in nestedTypes)
            {
                if (module != null && !string.Equals(type.Name, module, StringComparison.OrdinalIgnoreCase))
                    continue;

                var constants = type
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                    .Select(f => f.GetRawConstantValue()?.ToString());

                permissionList.AddRange(constants!);
            }

            return permissionList;
        }
    }
}
