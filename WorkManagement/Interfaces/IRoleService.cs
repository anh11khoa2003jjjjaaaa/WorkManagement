using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IRoleService
    {
        Task<Role> AddRoleAsync(Role role);
        Task<Role> UpdateRoleAsync(int id, Role role);
        Task DeleteRoleAsync(int id);
        Task<Role> GetRoleByIdAsync(int id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}
