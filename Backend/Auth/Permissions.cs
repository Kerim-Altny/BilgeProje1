namespace Backend.Auth;

public record PermissionDef(string Name, string Description, string Group);
public static class Permissions
{
    public const string ClaimType = "permission";

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

        new PermissionDef("Dashboard.Access",   "Panele Eriş",             "Dashboard"),
    };
}
