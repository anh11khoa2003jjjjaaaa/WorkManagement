using Microsoft.EntityFrameworkCore;
using WorkManagement.Data;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Service
{
    public class StatusJobService : IStatusJobService
    {
        private readonly WorkManagementContext _context;

        public StatusJobService(WorkManagementContext context)
        {
            _context = context;
        }

        // Thêm mới StatusJob
        public async Task<StatusJob> AddProjectStatusAsync(StatusJob projectStatus)
        {
            _context.StatusJobs.Add(projectStatus);
            await _context.SaveChangesAsync();
            return projectStatus;
        }

        // Cập nhật StatusJob
        public async Task<StatusJob> UpdateProjectStatusAsync(int id, StatusJob projectStatus)
        {
            var existingStatus = await _context.StatusJobs.FindAsync(id);
            if (existingStatus == null)
            {
                throw new KeyNotFoundException("StatusJob not found.");
            }

            existingStatus.Name = projectStatus.Name;
            existingStatus.Description = projectStatus.Description;

            await _context.SaveChangesAsync();
            return existingStatus;
        }

        // Xóa StatusJob
        public async Task DeleteProjectStatusAsync(int id)
        {
            var status = await _context.StatusJobs.FindAsync(id);
            if (status == null)
            {
                throw new KeyNotFoundException("StatusJob not found.");
            }

            _context.StatusJobs.Remove(status);
            await _context.SaveChangesAsync();
        }

        // Lấy StatusJob theo ID
        public async Task<StatusJob> GetProjectStatusByIdAsync(int id)
        {
            var status = await _context.StatusJobs.FindAsync(id);
            if (status == null)
            {
                throw new KeyNotFoundException("StatusJob not found.");
            }
            return status;
        }

        // Lấy tất cả các StatusJob
        public async Task<IEnumerable<StatusJob>> GetAllProjectStatusesAsync()
        {
            return await _context.StatusJobs.ToListAsync();
        }
    }
}
