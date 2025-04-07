using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkManagement.Data;
using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Service
{
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly WorkManagementContext _context;

        public ProjectTaskService(WorkManagementContext context)
        {
            _context = context;
        }

        // Hàm lưu hình ảnh vào thư mục và cơ sở dữ liệu
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

        // Thêm ProjectTask
        public async Task<ProjectTask> AddProjectTaskAsync(ProjectTaskDto projectTaskDto, List<IFormFile> images)
        {


            // Chuyển đổi ProjectDto thành đối tượng Project
            var projectTask = new ProjectTask
            {
                Name = projectTaskDto.Name,
                Description = projectTaskDto.Description,
                StatusId = projectTaskDto.StatusId,
                ProjectId = projectTaskDto.ProjectId,
               
            };

            // Lưu dự án vào cơ sở dữ liệu
            _context.ProjectTasks.Add(projectTask);
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
                        TaskId = projectTask.Id
                    };
                    _context.Images.Add(image);
                }

                await _context.SaveChangesAsync();
            }

            return projectTask;
        }
        public async Task<ProjectTask> UpdateProjectTaskAsync(int id, ProjectTaskDto projectTaskDto, List<IFormFile> images)
        {
            // Tìm Task hiện tại
            var existingTask = await _context.ProjectTasks
                .Include(pt => pt.Images)
                .FirstOrDefaultAsync(pt => pt.Id == id);
            if (existingTask == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            // Cập nhật thông tin Task
            existingTask.Name = projectTaskDto.Name;
            existingTask.Description = projectTaskDto.Description;
            existingTask.StatusId = projectTaskDto.StatusId;
            existingTask.ProjectId = projectTaskDto.ProjectId;

            // Xử lý hình ảnh

            if (images != null && images.Count > 0)
            {
                var fileUrls = await SaveImagesAsync(images);

                foreach (var url in fileUrls)
                {
                    var image = new Image
                    {
                        FilePath = url,
                        TaskId = existingTask.Id
                    };
                    _context.Images.Add(image);
                }
            }
            await _context.SaveChangesAsync();
            return existingTask;
        }
    
    

        // Xóa ProjectTask
        public async Task DeleteProjectTaskAsync(int id)
        {
            var task = await _context.ProjectTasks
                                     .Include(pt => pt.Images)
                                     .Include(pt => pt.TaskEmployees)
                                     .FirstOrDefaultAsync(pt => pt.Id == id);

            if (task == null)
            {
                throw new KeyNotFoundException("ProjectTask not found.");
            }

            // Xóa TaskEmployees
            if (task.TaskEmployees != null && task.TaskEmployees.Count > 0)
            {
                _context.TaskEmployees.RemoveRange(task.TaskEmployees);
            }

            // Xóa hình ảnh liên quan
            if (task.Images != null && task.Images.Count > 0)
            {
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                foreach (var image in task.Images)
                {
                    var filePath = Path.Combine(uploadDirectory, image.FilePath.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                _context.Images.RemoveRange(task.Images);
            }

            // Xóa Task
            _context.ProjectTasks.Remove(task);

            await _context.SaveChangesAsync();
        }

        // Lấy ProjectTask theo ID
        public async Task<ProjectTask> GetProjectTaskByIdAsync(int id)
        {
            var task = await _context.ProjectTasks
                                     .Include(pt => pt.Images)
                                     .Include(pt => pt.TaskEmployees)
                                     .FirstOrDefaultAsync(pt => pt.Id == id);

            if (task == null)
            {
                throw new KeyNotFoundException("ProjectTask not found.");
            }

            return task;
        }

        // Lấy ProjectTask theo tên
        public async Task<IEnumerable<ProjectTask>> GetProjectTasksByNameAsync(string name)
        {
            return await _context.ProjectTasks
                                 .Include(pt => pt.Images)
                                 .Include(pt => pt.TaskEmployees)
                                 .Where(pt => pt.Name.Contains(name))
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ProjectTask>> GetAll()
        {
            return await _context.ProjectTasks
                                 .Include(pt => pt.Images
                                 )
                                 .Include(pt => pt.Project)
                                 .Include(pt => pt.Status)
                                 .Include(pt => pt.TaskEmployees)
                                 .ToListAsync();
        }


        //Phân trang
        public async Task<PagedResult<ProjectTask>> GetAllPaged(int page, int pageSize)
        {
            var totalItems = await _context.ProjectTasks.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var projectTasks = await _context.ProjectTasks
                .Include(pt => pt.Images)
                .Include(pt => pt.Project)
                .Include(pt => pt.Status)
                .Include(pt => pt.TaskEmployees)
                .Skip((page - 1) * pageSize) // Bỏ qua các mục trước đó
                .Take(pageSize)             // Lấy số lượng mục theo pageSize
                .ToListAsync();

            return new PagedResult<ProjectTask>
            {
                Items = projectTasks,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksByProjectIdAsync(int id)
        {

            var projectExists = await _context.Projects.AnyAsync(p => p.Id == id);
            if (!projectExists)
            {
                throw new KeyNotFoundException("Project không tồn tại.");
            }
            return await _context.ProjectTasks
                .Include(task => task.Project)
                .Include(task => task.Status)
                // Load thông tin liên quan đến Project
                .Where(task => task.Project.Id == id) // Lọc theo ProjectId
                .ToListAsync(); // Trả về danh sách
        }


        //Lọc ra những công việc chưa phân công
        //public async Task<List<ProjectTask>> GetUnassignedTasksAsync()
        //{
        //    // Lấy tất cả các công việc đã được phân công
        //    var assignedTasks = await _context.TaskEmployees
        //                                      .Select(te => te.TaskId)
        //                                      .ToListAsync();

        //    // Lấy tất cả công việc trong cơ sở dữ liệu
        //    var allTasks = await _context.ProjectTasks.ToListAsync();

        //    // Lọc ra công việc chưa được phân công (không có trong danh sách đã phân công)
        //    var unassignedTasks = allTasks.Where(t => !assignedTasks.Contains(t.Id))
        //        
        //        .ToList();

        //    return unassignedTasks;
        //}
        public async Task<List<ProjectTask>> GetUnassignedTasksAsync()
        {
            // Lấy danh sách TaskId đã được phân công
            var assignedTaskIds = await _context.TaskEmployees
                                                .Select(te => te.TaskId)
                                                .ToListAsync();

            // Lấy danh sách công việc chưa được phân công và bao gồm chi tiết Project
            var unassignedTasks = await _context.ProjectTasks
                                                .Where(t => !assignedTaskIds.Contains(t.Id))
                                                .Include(t => t.Project) // Bao gồm chi tiết dự án
                                                .ToListAsync();

            return unassignedTasks;
        }


    }

}
