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

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<User>()
        .HasOne(u => u.Role)
        .WithMany(r => r.Users)
        .HasForeignKey(u => u.RoleId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "SuperAdmin", CanAdd = true, CanEdit = true, CanDelete = true, CanAccessDashboard = true },
            new Role { Id = 2, Name = "Admin", CanAdd = true, CanEdit = true, CanDelete = true, CanAccessDashboard = true },
            new Role { Id = 3, Name = "Editor", CanAdd = true, CanEdit = true, CanDelete = false, CanAccessDashboard = true },
            new Role { Id = 4, Name = "User", CanAdd = false, CanEdit = false, CanDelete = false, CanAccessDashboard = true }
        );
    }

}