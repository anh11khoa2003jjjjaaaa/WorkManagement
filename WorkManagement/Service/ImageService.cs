using Microsoft.EntityFrameworkCore;
using WorkManagement.Data;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Service
{
    public class ImageService : IImageService
    {
        private readonly WorkManagementContext _context;

        public ImageService(WorkManagementContext context)
        {
            _context = context;
        }

        public async Task AddImageAsync(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "Image cannot be null.");
            }


            _context.Images.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveImageAsync(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) throw new KeyNotFoundException($"Image with ID {id} not found.");

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
        }

        public async Task<Image?> GetImageByIdAsync(int id)
        {
            return await _context.Images
                .Include(i => i.Project)
                .Include(i => i.Task)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Image>> GetImagesByProjectIdAsync(int projectId)
        {
            return await _context.Images
                .Where(i => i.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Image>> GetImagesByTaskIdAsync(int taskId)
        {
            return await _context.Images
                .Where(i => i.TaskId == taskId)
                .ToListAsync();
        }
    }
}
