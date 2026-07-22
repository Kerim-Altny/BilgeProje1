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

    public DbSet<ShortLink> ShortLinks { get; set; }
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

        modelBuilder.Entity<ShortLink>().HasIndex(l => l.CreatedByUserId);
        modelBuilder.Entity<ShortLink>().HasIndex(l => l.ShortCode).IsUnique();

        modelBuilder.Entity<ShortLink>().HasOne(l => l.CreatedByUser)
            .WithMany()
            .HasForeignKey(l => l.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Name = "Users.View", Description = "Kullanıcıları Görüntüle", Group = "Users" },
            new Permission { Id = 2, Name = "Users.Create", Description = "Kullanıcı Ekle", Group = "Users" },
            new Permission { Id = 3, Name = "Users.Edit", Description = "Kullanıcı Düzenle", Group = "Users" },
            new Permission { Id = 4, Name = "Users.Delete", Description = "Kullanıcı Sil", Group = "Users" },
            new Permission { Id = 5, Name = "Roles.View", Description = "Rolleri Görüntüle", Group = "Roles" },
            new Permission { Id = 6, Name = "Roles.Create", Description = "Rol Ekle", Group = "Roles" },
            new Permission { Id = 7, Name = "Roles.Edit", Description = "Rol Düzenle", Group = "Roles" },
            new Permission { Id = 8, Name = "Roles.Delete", Description = "Rol Sil", Group = "Roles" },
            new Permission { Id = 9, Name = "Dashboard.Access", Description = "Panele Eriş", Group = "Dashboard" },
            new Permission { Id = 10, Name = "Links.View", Description = "Linkleri Görüntüle", Group = "Links" },
            new Permission { Id = 11, Name = "Links.Create", Description = "Link Oluştur", Group = "Links" },
            new Permission { Id = 12, Name = "Links.Edit", Description = "Link Düzenle", Group = "Links" },
            new Permission { Id = 13, Name = "Links.Delete", Description = "Link Sil", Group = "Links" },
            new Permission { Id = 14, Name = "Links.ManageAll", Description = "Tüm Kullanıcıların Linklerini Yönet", Group = "Links" }

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
            new RolePermission { RoleId = 1, PermissionId = 12 },
            new RolePermission { RoleId = 1, PermissionId = 13 },
            new RolePermission { RoleId = 1, PermissionId = 14 },

            // Admin: Users + Roles hepsi + Dashboard
            new RolePermission { RoleId = 2, PermissionId = 1 },
            new RolePermission { RoleId = 2, PermissionId = 2 },
            new RolePermission { RoleId = 2, PermissionId = 3 },
            new RolePermission { RoleId = 2, PermissionId = 4 },
            new RolePermission { RoleId = 2, PermissionId = 5 },
            new RolePermission { RoleId = 2, PermissionId = 6 },
            new RolePermission { RoleId = 2, PermissionId = 7 },
            new RolePermission { RoleId = 2, PermissionId = 8 },
            new RolePermission { RoleId = 2, PermissionId = 9 },
            new RolePermission { RoleId = 2, PermissionId = 10 },
            new RolePermission { RoleId = 2, PermissionId = 11 },
            new RolePermission { RoleId = 2, PermissionId = 12 },
            new RolePermission { RoleId = 2, PermissionId = 13 },
            new RolePermission { RoleId = 2, PermissionId = 14 },

            // Editor: Users.View/Create/Edit + Dashboard
            new RolePermission { RoleId = 3, PermissionId = 1 },
            new RolePermission { RoleId = 3, PermissionId = 2 },
            new RolePermission { RoleId = 3, PermissionId = 3 },
            new RolePermission { RoleId = 3, PermissionId = 9 },
            new RolePermission { RoleId = 3, PermissionId = 10 },
            new RolePermission { RoleId = 3, PermissionId = 11 },
            new RolePermission { RoleId = 3, PermissionId = 12 },
            new RolePermission { RoleId = 3, PermissionId = 13 }


            // User: Temel kullanıcı işlemleri izne bağlı değildir (sadece Authorize gerektirir)
            // Bu nedenle RoleId = 4 için hiçbir özel izin (Dashboard, Links.* vs.) atamıyoruz.
        );
    }
}
