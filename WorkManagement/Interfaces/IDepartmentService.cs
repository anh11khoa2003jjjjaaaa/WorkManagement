using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IDepartmentService
    {
        Task<Department> AddDepartmentAsync(Department department);
        Task<Department> UpdateDepartmentAsync(int id, Department department);
        Task DeleteDepartmentAsync(int id);
        Task<Department> GetDepartmentByIdAsync(int id);
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();

        // Thêm phương thức để lấy nhân viên theo phòng ban
        Task<IEnumerable<object>> GetEmployeesByDepartmentIdAsync(int departmentId);

    }
}
