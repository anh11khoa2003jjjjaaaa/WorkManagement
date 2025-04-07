using WorkManagement.DTO;
using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IProjectTaskService
    {
        Task<ProjectTask> AddProjectTaskAsync(ProjectTaskDto projectTaskDto, List<IFormFile> images);
        Task<ProjectTask> UpdateProjectTaskAsync(int id, ProjectTaskDto projectTaskDto, List<IFormFile> images);
        Task DeleteProjectTaskAsync(int id);
        Task<ProjectTask> GetProjectTaskByIdAsync(int id);
        Task<IEnumerable<ProjectTask>> GetProjectTasksByNameAsync(string name);
        Task<IEnumerable<ProjectTask>> GetAll();

        Task<IEnumerable<ProjectTask>> GetTasksByProjectIdAsync(int id);
        Task<PagedResult<ProjectTask>> GetAllPaged(int page, int pageSize);
        Task<List<ProjectTask>> GetUnassignedTasksAsync();

    }
}