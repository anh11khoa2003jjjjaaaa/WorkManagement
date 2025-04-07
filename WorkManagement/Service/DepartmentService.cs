using WorkManagement.Data;
using WorkManagement.Interfaces;
using WorkManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkManagement.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly WorkManagementContext _context;

        public DepartmentService(WorkManagementContext context)
        {
            _context = context;
        }

        public async Task<Department> AddDepartmentAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<Department> UpdateDepartmentAsync(int id, Department department)
        {
            var existingDepartment = await _context.Departments.FindAsync(id);
            if (existingDepartment == null)
            {
                throw new KeyNotFoundException("Department not found.");
            }

            existingDepartment.Name = department.Name;
            existingDepartment.Description = department.Description;

            await _context.SaveChangesAsync();
            return existingDepartment;
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await GetDepartmentByIdAsync(id);
            if (department == null)
            {
                throw new KeyNotFoundException("Department not found.");
            }
           
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Projects)
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                throw new KeyNotFoundException("Department not found.");
            }

            return department;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .Include(d => d.Projects)
                .Include(d => d.Users)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetEmployeesByDepartmentIdAsync(int departmentId)
        {
                            var employees = await _context.Users
                     .Where(u => u.DepartmentId == departmentId)
                     .Select(u => new
                     {
                         EmployeeId = u.Id,
                         EmployeeName = u.Name,
                         u.Email,
                         u.Phone,
                         u.BirthDay,
                         u.Avatar,
                         DepartmentName = u.Department.Name
                     })
                     .ToListAsync();

            return employees;

        }

    }
}