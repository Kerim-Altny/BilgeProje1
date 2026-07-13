using AutoMapper;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PermissionService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync()
    {
        var permissions = await _context.Permissions.ToListAsync();
        return _mapper.Map<List<PermissionDto>>(permissions);
    }

    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsByRoleIdAsync(int roleId)
    {
        var permissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission!)
            .ToListAsync();

        return _mapper.Map<List<PermissionDto>>(permissions);
    }

    public async Task<bool> SetPermissionsForRoleAsync(int roleId, IEnumerable<int> permissionIds)
    {
        var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
        if (!roleExists) return false;

        var existing = _context.RolePermissions.Where(rp => rp.RoleId == roleId);
        _context.RolePermissions.RemoveRange(existing);

        foreach (var permissionId in permissionIds.Distinct())
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
