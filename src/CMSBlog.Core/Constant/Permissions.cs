using System.ComponentModel;

namespace CMSBlog.Core.Constant
{
    public static class Permissions
    {
        public static class Dashboard
        {
            [Description("Permission to view the dashboard")]
            public const string View = "Permissions.Dashboard.View";
        }

        public static class Roles
        {
            [Description("Permission to view roles")]
            public const string View = "Permissions.Roles.View";
            [Description("Permission to create roles")]
            public const string Create = "Permissions.Roles.Create";
            [Description("Permission to edit roles")]
            public const string Edit = "Permissions.Roles.Edit";
            [Description("Permission to delete roles")]
            public const string Delete = "Permissions.Roles.Delete";
        }

        public static class Users
        {
            [Description("Permission to view users")]
            public const string View = "Permissions.Users.View";
            [Description("Permission to create users")]
            public const string Create = "Permissions.Users.Create";
            [Description("Permission to edit users")]
            public const string Edit = "Permissions.Users.Edit";
            [Description("Permission to delete users")]
            public const string Delete = "Permissions.Users.Delete";
        }
    }
}
