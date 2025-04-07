using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorkManagement.Data;
using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Services
{
    public class ProjectService : IProjectService
    {
        private readonly WorkManagementContext _context;

        public ProjectService(WorkManagementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hàm lưu hình ảnh vào thư mục và trả về danh sách URL lưu trữ.
        /// </summary>
        private async Task<List<string>> SaveImagesAsync(List<IFormFile> images)
        {
            var fileUrls = new List<string>();
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    var fileExtension = Path.GetExtension(image.FileName);
                    var fileName = $"{Guid.NewGuid()}_{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var fileUrl = $"/Uploads/{fileName}";
                    fileUrls.Add(fileUrl);
                }
            }

            return fileUrls;
        }

        /// <summary>
        /// Thêm dự án mới cùng với hình ảnh.
        /// </summary>
        public async Task<Project> AddProjectAsync(ProjectDto projectDto, List<IFormFile> images)
        {
            
           
                // Chuyển đổi ProjectDto thành đối tượng Project
                var project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                StartDate = projectDto.StartDate,
                EndDate = projectDto.EndDate,
                StatusId = projectDto.StatusId,
                ManagerId = projectDto.ManagerId,
                DepartmentId = projectDto.DepartmentId
                };

            // Lưu dự án vào cơ sở dữ liệu
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Lưu hình ảnh nếu có
            if (images != null && images.Count > 0)
            {
                var fileUrls = await SaveImagesAsync(images);

                foreach (var url in fileUrls)
                {
                    var image = new Image
                    {
                        FilePath = url,
                        ProjectId = project.Id
                    };
                    _context.Images.Add(image);
                }

                await _context.SaveChangesAsync();
            }

            return project;
        }


        /// <summary>
        /// Cập nhật thông tin dự án và thêm hình ảnh mới nếu có.
        /// </summary>
        public async Task<Project> UpdateProjectAsync(int id, ProjectDto projectDto, List<IFormFile> images)
        {
            var existingProject = await _context.Projects.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (existingProject == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            // Cập nhật thông tin dự án
            existingProject.Name = projectDto.Name;
            existingProject.Description = projectDto.Description;
            existingProject.StartDate = projectDto.StartDate;
            existingProject.EndDate = projectDto.EndDate;
            existingProject.StatusId = projectDto.StatusId;
            existingProject.ManagerId = projectDto.ManagerId;
            existingProject.DepartmentId = projectDto.DepartmentId;


            // Xử lý hình ảnh mới nếu có
            if (images != null && images.Count > 0)
            {
                var fileUrls = await SaveImagesAsync(images);

                foreach (var url in fileUrls)
                {
                    var image = new Image
                    {
                        FilePath = url,
                        ProjectId = existingProject.Id
                    };
                    _context.Images.Add(image);
                }
            }

            await _context.SaveChangesAsync();
            return existingProject;
        }


        /// <summary>
        /// Xóa dự án và các hình ảnh liên quan.
        /// </summary>
        public async Task DeleteProjectAsync(int id)
        {
            var project = await _context.Projects
                                        .Include(p => p.Images) // Hình ảnh liên kết với Project
                                        .Include(p => p.ProjectTasks) // Nhiệm vụ liên kết với Project
                                            .ThenInclude(t => t.Images) // Hình ảnh liên kết với Task
                                        .Include(p => p.ProjectTasks)
                                            .ThenInclude(t => t.TaskEmployees) // Nhân viên liên kết với Task
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            // Xóa Images liên quan đến các ProjectTasks
            foreach (var task in project.ProjectTasks)
            {
                if (task.Images != null && task.Images.Count > 0)
                {
                    foreach (var image in task.Images)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", image.FilePath.TrimStart('/'));
                        if (File.Exists(filePath))
                        {
                            try
                            {
                                File.Delete(filePath);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Error deleting image file: {ex.Message}");
                            }
                        }
                    }
                    _context.Images.RemoveRange(task.Images);
                }

                // Xóa TaskEmployees
                if (task.TaskEmployees != null && task.TaskEmployees.Count > 0)
                {
                    _context.TaskEmployees.RemoveRange(task.TaskEmployees);
                }
            }

            // Xóa ProjectTasks
            if (project.ProjectTasks != null && project.ProjectTasks.Count > 0)
            {
                _context.ProjectTasks.RemoveRange(project.ProjectTasks);
            }

            // Xóa Images liên kết với Project
            if (project.Images != null && project.Images.Count > 0)
            {
                foreach (var image in project.Images)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", image.FilePath.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error deleting image file: {ex.Message}");
                        }
                    }
                }
                _context.Images.RemoveRange(project.Images);
            }

            // Xóa Project
            _context.Projects.Remove(project);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException?.Message ?? "No additional information.";
                throw new Exception($"Database error while deleting project: {innerException}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting project: {ex.Message}");
            }
        }



        /// <summary>
        /// Lấy thông tin dự án theo ID, bao gồm hình ảnh.
        /// </summary>
        public async Task<Project> GetProjectByIdAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Images)
                .Include(c => c.Department)
                .Include(e => e.ProjectTasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            return project;
        }

        /// <summary>
        /// Lấy danh sách tất cả dự án, bao gồm hình ảnh.
        /// </summary>
        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.Images)
                .Include(s => s.Status)
                .Include(d => d.Department)
                .Include(u => u.Manager)
                .ToListAsync();
        }

        //Phân trang
        public async Task<PagedResult<Project>> GetAllPageProjectsAsync(int page, int pageSize)
        {
            var totalItems = await _context.Projects.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var projects = await _context.Projects
                .Include(p => p.Images)
                .Include(s => s.Status)
                .Include(d => d.Department)
                .Include(u => u.Manager)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Project>
            {
                Items = projects,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<List<Project>> GetProjectsByDepartmentIdAsync(int departmentId)
        {
            var projects = await _context.Projects
                .Include(p => p.Images)
                .Include(p => p.ProjectTasks)
                .Include(d => d.Department)
                .Include(u => u.Manager)
                .Include(u => u.Status)
                .Where(p => p.DepartmentId == departmentId)
                .ToListAsync();

            if (!projects.Any())
            {
                throw new KeyNotFoundException("No projects found for the specified department.");
            }

            return projects;
        }

    }
}
