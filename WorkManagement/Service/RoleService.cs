using Microsoft.EntityFrameworkCore;
using WorkManagement.Data;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Service
{
    public class RoleService : IRoleService
    {
        private readonly WorkManagementContext _context;

        public RoleService(WorkManagementContext context)
        {
            _context = context;
        }

        // Thêm mới Role
        public async Task<Role> AddRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        // Cập nhật Role
        public async Task<Role> UpdateRoleAsync(int id, Role role)
        {
            var existingRole = await _context.Roles.FindAsync(id);
            if (existingRole == null)
            {
                throw new KeyNotFoundException("Role not found.");
            }

            existingRole.Name = role.Name;
           

            await _context.SaveChangesAsync();
            return existingRole;
        }

        // Xóa Role
        public async Task DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                throw new KeyNotFoundException("Role not found.");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }

        // Lấy Role theo ID
        public async Task<Role> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                throw new KeyNotFoundException("Role not found.");
            }
            return role;
        }

        // Lấy tất cả Roles
        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
