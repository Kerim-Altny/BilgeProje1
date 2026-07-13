using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<User>()
        .HasOne(u => u.Role)
        .WithMany(r => r.Users)
        .HasForeignKey(u => u.RoleId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "SuperAdmin" },
            new Role { Id = 2, Name = "Admin" },
            new Role { Id = 3, Name = "Editor" },
            new Role { Id = 4, Name = "User" }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "Users.View", Description = "Kullanıcıları Görüntüle", Group = "Users" },
            new Permission { Id = 2, Name = "Users.Create", Description = "Kullanıcı Ekle", Group = "Users" },
            new Permission { Id = 3, Name = "Users.Edit", Description = "Kullanıcı Düzenle", Group = "Users" },
            new Permission { Id = 4, Name = "Users.Delete", Description = "Kullanıcı Sil", Group = "Users" },
            new Permission { Id = 5, Name = "Roles.View", Description = "Rolleri Görüntüle", Group = "Roles" },
            new Permission { Id = 6, Name = "Roles.Create", Description = "Rol Ekle", Group = "Roles" },
            new Permission { Id = 7, Name = "Roles.Edit", Description = "Rol Düzenle", Group = "Roles" },
            new Permission { Id = 8, Name = "Roles.Delete", Description = "Rol Sil", Group = "Roles" },
            new Permission { Id = 9, Name = "Permissions.View", Description = "Yetkileri Görüntüle", Group = "Permissions" },
            new Permission { Id = 10, Name = "Permissions.Assign", Description = "Yetki Ata", Group = "Permissions" },
            new Permission { Id = 11, Name = "Dashboard.Access", Description = "Panele Eriş", Group = "Dashboard" }
        );

        modelBuilder.Entity<RolePermission>().HasData(
            // SuperAdmin: hepsi
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            new RolePermission { RoleId = 1, PermissionId = 4 },
            new RolePermission { RoleId = 1, PermissionId = 5 },
            new RolePermission { RoleId = 1, PermissionId = 6 },
            new RolePermission { RoleId = 1, PermissionId = 7 },
            new RolePermission { RoleId = 1, PermissionId = 8 },
            new RolePermission { RoleId = 1, PermissionId = 9 },
            new RolePermission { RoleId = 1, PermissionId = 10 },
            new RolePermission { RoleId = 1, PermissionId = 11 },

            // Admin: Users + Roles hepsi + Dashboard
            new RolePermission { RoleId = 2, PermissionId = 1 },
            new RolePermission { RoleId = 2, PermissionId = 2 },
            new RolePermission { RoleId = 2, PermissionId = 3 },
            new RolePermission { RoleId = 2, PermissionId = 4 },
            new RolePermission { RoleId = 2, PermissionId = 5 },
            new RolePermission { RoleId = 2, PermissionId = 6 },
            new RolePermission { RoleId = 2, PermissionId = 7 },
            new RolePermission { RoleId = 2, PermissionId = 8 },
            new RolePermission { RoleId = 2, PermissionId = 11 },

            // Editor: Users.View/Create/Edit + Dashboard
            new RolePermission { RoleId = 3, PermissionId = 1 },
            new RolePermission { RoleId = 3, PermissionId = 2 },
            new RolePermission { RoleId = 3, PermissionId = 3 },
            new RolePermission { RoleId = 3, PermissionId = 11 },

            // User: sadece Dashboard.Access
            new RolePermission { RoleId = 4, PermissionId = 11 }
        );
    }
}
