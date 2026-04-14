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

        public static class PostCategories
        {
            [Description("Permission to view post categories")]
            public const string View = "Permissions.PostCategories.View";
            [Description("Permission to create post categories")]
            public const string Create = "Permissions.PostCategories.Create";
            [Description("Permission to edit post categories")]
            public const string Edit = "Permissions.PostCategories.Edit";
            [Description("Permission to delete post categories")]
            public const string Delete = "Permissions.UsPostCategoriesers.Delete";
        }
        public static class Posts
        {
            [Description("Permission to view posts")]
            public const string View = "Permissions.Posts.View";
            [Description("Permission to create posts")]
            public const string Create = "Permissions.Posts.Create";
            [Description("Permission to edit posts")]
            public const string Edit = "Permissions.Posts.Edit";
            [Description("Permission to delete posts")]
            public const string Delete = "Permissions.Posts.Delete";
            [Description("Permission to approve posts")]
            public const string Approve = "Permissions.Posts.Approve";
        }

        public static class Series
        {
            [Description("Permission to view series")]
            public const string View = "Permissions.Series.View";
            [Description("Permission to create series")]
            public const string Create = "Permissions.Series.Create";
            [Description("Permission to edit series")]
            public const string Edit = "Permissions.Series.Edit";
            [Description("Permission to delete series")]
            public const string Delete = "Permissions.Series.Delete";
        }
        public static class Royalty
        {
            [Description("Permission to view royalty")]
            public const string View = "Permissions.Royalty.View";
            [Description("Permission to pay royalty")]
            public const string Pay = "Permissions.Royalty.Pay";
        }
    }
}
