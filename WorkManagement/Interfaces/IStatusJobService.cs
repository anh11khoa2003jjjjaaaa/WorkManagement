using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IStatusJobService
    {
        Task<StatusJob> AddProjectStatusAsync(StatusJob projectStatus);
        Task<StatusJob> UpdateProjectStatusAsync(int id, StatusJob projectStatus);
        Task DeleteProjectStatusAsync(int id);
        Task<StatusJob> GetProjectStatusByIdAsync(int id);
        Task<IEnumerable<StatusJob>> GetAllProjectStatusesAsync();
    }
}
