using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IImageService
    {
        Task AddImageAsync(Image image);
        Task RemoveImageAsync(int id);
        Task<Image?> GetImageByIdAsync(int id);
        Task<IEnumerable<Image>> GetImagesByProjectIdAsync(int projectId);
        Task<IEnumerable<Image>> GetImagesByTaskIdAsync(int taskId);
        
    }
}
