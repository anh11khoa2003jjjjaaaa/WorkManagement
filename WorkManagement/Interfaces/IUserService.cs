using WorkManagement.DTO;
using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IUserService
    {
        Task<User> AddUserAsync(User user, IFormFile avatarFile);
        Task<User> UpdateUserAsync(int id, User user, IFormFile avatarFile);
        Task DeleteUserAsync(int id);
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetUsersByNameAsync(string name);
        Task<User> UpdateAvatarAsync(int id, IFormFile avatarFile);

        Task UpdateDepartmentAsync(int userId, int departmentId);

        Task<IEnumerable<User>> GetUserByIsLeader();

        //Task<IEnumerable<User>> GetAll();
        Task<IEnumerable<object>> GetAll();
        Task<List<User>> GetEmployeesForTaskAsync(int taskId);
        Task<User> GetUserbyAccountId(int accountId);

        // Thống kê danh sách nhân viên thuộc phòng ban

        Task<IEnumerable<object>> StatisticalEmployeeOfDepartment();
        //Thống kê danh sách có bao nhiêu dự án làm xong có bao nhiêu dự án chua làm
        Task<IEnumerable<object>> GetProjectStatusStatistics();

        //Thống kê danh sách nhân viên làm đúng hạn, trễ hạn trong 1 tháng
        Task<object> GetEmployeeTaskCompletionStatusWithCount();
        //Thống kê danh sách công việc thuộc 1 dự án 
        Task<IEnumerable<object>> GetProjectTaskCountsAsync();
        Task<object> GetAllPaged(int page, int pageSize);
    }
}
