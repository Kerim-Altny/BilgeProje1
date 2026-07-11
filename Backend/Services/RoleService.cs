using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RoleResponse>> GetAllRolesAsync()
    {
        return await _context.Roles
            .Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name,
                CanAdd = r.CanAdd,
                CanEdit = r.CanEdit,
                CanDelete = r.CanDelete,
                CanAccessDashboard = r.CanAccessDashboard
            })
            .ToListAsync();
    }

    public async Task<RoleResponse?> GetRoleByIdAsync(int roleId)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role is null) return null;

        return new RoleResponse
        {
            Id = role.Id,
            Name = role.Name,
            CanAdd = role.CanAdd,
            CanEdit = role.CanEdit,
            CanDelete = role.CanDelete,
            CanAccessDashboard = role.CanAccessDashboard
        };
    }

    public async Task<RoleResult> CreateRoleAsync(RoleCreateRequest request)
    {
        if (await _context.Roles.AnyAsync(r => r.Name.ToLower() == request.Name.ToLower()))
        {
            return new RoleResult { Status = RoleResultStatus.Conflict, Message = "Bu rol adı zaten kullanılıyor." };
        }

        var newRole = new Role
        {
            Name = request.Name,
            CanAdd = request.CanAdd,
            CanEdit = request.CanEdit,
            CanDelete = request.CanDelete,
            CanAccessDashboard = request.CanAccessDashboard
        };

        _context.Roles.Add(newRole);
        await _context.SaveChangesAsync();

        return new RoleResult
        {
            Status = RoleResultStatus.Success,
            Data = new RoleResponse
            {
                Id = newRole.Id,
                Name = newRole.Name,
                CanAdd = newRole.CanAdd,
                CanEdit = newRole.CanEdit,
                CanDelete = newRole.CanDelete,
                CanAccessDashboard = newRole.CanAccessDashboard
            }
        };
    }

    public async Task<RoleResult> UpdateRoleAsync(int roleId, RoleUpdateRequest request)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role is null) return new RoleResult { Status = RoleResultStatus.NotFound };

        if (!string.IsNullOrEmpty(request.Name) && 
            await _context.Roles.AnyAsync(r => r.Id != roleId && r.Name.ToLower() == request.Name.ToLower()))
        {
            return new RoleResult { Status = RoleResultStatus.Conflict, Message = "Bu rol adı zaten kullanılıyor." };
        }

        if (request.Name != null)
            role.Name = request.Name;

        if (request.CanAdd.HasValue)
            role.CanAdd = request.CanAdd.Value;

        if (request.CanEdit.HasValue)
            role.CanEdit = request.CanEdit.Value;

        if (request.CanDelete.HasValue)
            role.CanDelete = request.CanDelete.Value;

        if (request.CanAccessDashboard.HasValue)
            role.CanAccessDashboard = request.CanAccessDashboard.Value;

        await _context.SaveChangesAsync();

        return new RoleResult
        {
            Status = RoleResultStatus.Success,
            Data = new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                CanAdd = role.CanAdd,
                CanEdit = role.CanEdit,
                CanDelete = role.CanDelete,
                CanAccessDashboard = role.CanAccessDashboard
            }
        };
    }

    public async Task<bool> DeleteRoleAsync(int roleId)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role is null) return false;

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }
}
