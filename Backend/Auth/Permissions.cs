namespace Backend.Auth;

public record PermissionDef(string Name, string Description, string Group);
public static class Permissions
{
    public const string ClaimType = "permission";

    public const string CanAdd = "CanAdd";
    public const string CanEdit = "CanEdit";
    public const string CanDelete = "CanDelete";
    public const string CanAccessDashboard = "CanAccessDashboard";

    public static readonly IReadOnlyList<PermissionDef> All = new[]
    {
        new PermissionDef("Users.View",         "Kullanıcıları Görüntüle", "Users"),
        new PermissionDef("Users.Create",       "Kullanıcı Ekle",          "Users"),
        new PermissionDef("Users.Edit",         "Kullanıcı Düzenle",       "Users"),
        new PermissionDef("Users.Delete",       "Kullanıcı Sil",           "Users"),
        new PermissionDef("Roles.View",         "Rolleri Görüntüle",       "Roles"),
        new PermissionDef("Roles.Create",       "Rol Ekle",                "Roles"),
        new PermissionDef("Roles.Edit",         "Rol Düzenle",             "Roles"),
        new PermissionDef("Roles.Delete",       "Rol Sil",                 "Roles"),
        new PermissionDef("Permissions.View",   "Yetkileri Görüntüle",     "Permissions"),
        new PermissionDef("Permissions.Assign", "Yetki Ata",               "Permissions"),
        new PermissionDef("Dashboard.Access",   "Panele Eriş",             "Dashboard"),
    };
}
