using AutoMapper;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RoleService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<RoleResponse>> GetAllRolesAsync()
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .ToListAsync();
        return _mapper.Map<List<RoleResponse>>(roles);
    }

    public async Task<RoleResponse?> GetRoleByIdAsync(int roleId)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId);
        if (role is null) return null;

        return _mapper.Map<RoleResponse>(role);
    }

    public async Task<Result<RoleResponse>> CreateRoleAsync(RoleCreateRequest request)
    {
        if (await _context.Roles.AnyAsync(r => r.Name.ToLower() == request.Name.ToLower()))
        {
            return Result<RoleResponse>.Conflict("Bu rol adı zaten kullanılıyor.");
        }

        var newRole = _mapper.Map<Role>(request);

        var validPermissions = await _context.Permissions
            .Where(p => request.Permissions.Contains(p.Name))
            .ToListAsync();

        foreach (var permission in validPermissions)
        {
            newRole.RolePermissions.Add(new RolePermission { Permission = permission });
        }
        _context.Roles.Add(newRole);
        await _context.SaveChangesAsync();

        return Result<RoleResponse>.Ok(_mapper.Map<RoleResponse>(newRole));
    }

    public async Task<Result<RoleResponse>> UpdateRoleAsync(int roleId, RoleUpdateRequest request)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId);
        if (role is null) return Result<RoleResponse>.NotFound();

        if (!string.IsNullOrEmpty(request.Name) &&
            await _context.Roles.AnyAsync(r => r.Id != roleId && r.Name.ToLower() == request.Name.ToLower()))
        {
            return Result<RoleResponse>.Conflict("Bu rol adı zaten kullanılıyor.");
        }

        if (request.Name != null)
            role.Name = request.Name;

        if (request.Permissions != null)
        {
            _context.RolePermissions.RemoveRange(role.RolePermissions);

            var validPermissions = await _context.Permissions
                .Where(p => request.Permissions.Contains(p.Name))
                .ToListAsync();

            foreach (var permission in validPermissions)
            {
                role.RolePermissions.Add(new RolePermission { Permission = permission });
            }
        }

        await _context.SaveChangesAsync();

        return Result<RoleResponse>.Ok(_mapper.Map<RoleResponse>(role));
    }

    public async Task<Result<bool>> DeleteRoleAsync(int roleId)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role is null) return Result<bool>.NotFound();

        if (await _context.Users.AnyAsync(u => u.RoleId == roleId))
        {
            return Result<bool>.Conflict("Bu rol kullanıcılara atanmış olduğu için silinemez.");
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> DeleteRolesAsync(IEnumerable<int> roleIds)
    {
        var roles = await _context.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
        if (!roles.Any()) return Result<bool>.NotFound();

        if (await _context.Users.AnyAsync(u => roleIds.Contains(u.RoleId)))
        {
            return Result<bool>.Conflict("Seçili rollerden bazıları kullanıcılara atanmış olduğu için silme işlemi iptal edildi.");
        }

        _context.Roles.RemoveRange(roles);
        await _context.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }
}
