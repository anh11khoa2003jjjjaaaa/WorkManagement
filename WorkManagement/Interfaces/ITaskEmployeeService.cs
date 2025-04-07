using WorkManagement.DTO;
using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface ITaskEmployeeService
    {

        Task<TaskEmployee> AddTaskEmployeeAsync(TaskEmployee taskEmployee);
        Task<TaskEmployee> UpdateTaskEmployeeAsync(int id, UpdateTaskEmployeeDto updateTaskEmployeeDto);
        Task DeleteTaskEmployeeAsync(int id);
        Task<TaskEmployee> GetTaskEmployeeByIdAsync(int id);
        Task<IEnumerable<TaskEmployee>> GetTaskEmployeesByTaskNameAsync(string taskName);
        Task<IEnumerable<TaskEmployee>> GetTaskEmployeesByUserNameAsync(string userName);
        Task<IEnumerable<TaskEmployeeDto>> GetAllTaskEmployeesAsync();
        Task UpdateTaskStatusAsync(int taskEmployeeId, int newStatusId); // Cập nhật trạng thái công việc
        Task CheckAndUpdateProjectStatusAsync(int? projectId); // Kiểm tra và cập nhật trạng thái dự án

        Task <IEnumerable<TaskEmployee>> GetTaskEmployeeByUserId(int userId);

        Task<PagedResult<TaskEmployeeDto>> GetPagedTaskEmployeesAsync(int pageNumber, int pageSize);


    }
}
