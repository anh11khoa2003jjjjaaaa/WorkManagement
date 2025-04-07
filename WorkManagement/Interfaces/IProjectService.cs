using WorkManagement.DTO;
using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IProjectService
    {
        Task<Project> AddProjectAsync(ProjectDto project, List<IFormFile> images);
        Task<Project> UpdateProjectAsync(int id, ProjectDto projectDto, List<IFormFile> images);
        Task DeleteProjectAsync(int id);
        Task<Project> GetProjectByIdAsync(int id);
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<List<Project>> GetProjectsByDepartmentIdAsync(int departmentId);
        Task<PagedResult<Project>> GetAllPageProjectsAsync(int page, int pageSize);
    }
}
