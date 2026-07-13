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
        var roles = await _context.Roles.ToListAsync();
        return _mapper.Map<List<RoleResponse>>(roles);
    }

    public async Task<RoleResponse?> GetRoleByIdAsync(int roleId)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role is null) return null;

        return _mapper.Map<RoleResponse>(role);
    }

    public async Task<RoleResult> CreateRoleAsync(RoleCreateRequest request)
    {
        if (await _context.Roles.AnyAsync(r => r.Name.ToLower() == request.Name.ToLower()))
        {
            return new RoleResult { Status = RoleResultStatus.Conflict, Message = "Bu rol adı zaten kullanılıyor." };
        }

        var newRole = _mapper.Map<Role>(request);

        _context.Roles.Add(newRole);
        await _context.SaveChangesAsync();

        return new RoleResult
        {
            Status = RoleResultStatus.Success,
            Data = _mapper.Map<RoleResponse>(newRole)
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

        await _context.SaveChangesAsync();

        return new RoleResult
        {
            Status = RoleResultStatus.Success,
            Data = _mapper.Map<RoleResponse>(role)
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
