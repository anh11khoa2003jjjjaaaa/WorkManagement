using Microsoft.EntityFrameworkCore;
using WorkManagement.Data;
using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Service
{
    public class TaskEmployeeService : ITaskEmployeeService
    {
        private readonly WorkManagementContext _context;

        public TaskEmployeeService(WorkManagementContext context)
        {
            _context = context;
        }

        // Thêm mới TaskEmployee
        public async Task<TaskEmployee> AddTaskEmployeeAsync(TaskEmployee taskEmployee)
        {
            try
            {
                _context.TaskEmployees.Add(taskEmployee);
                await _context.SaveChangesAsync();
                return taskEmployee;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                throw new Exception("An error occurred while saving the task employee.", ex);
            }
        }
      

        public async Task<TaskEmployee> UpdateTaskEmployeeAsync(int id, UpdateTaskEmployeeDto updateTaskEmployeeDto)
        {
            // Tìm TaskEmployee trong cơ sở dữ liệu
            var existingTaskEmployee = await _context.TaskEmployees.FindAsync(id);
            if (existingTaskEmployee == null)
            {
                throw new KeyNotFoundException("TaskEmployee not found.");
            }

            // Cập nhật các trường từ DTO
            if (updateTaskEmployeeDto.TaskId.HasValue)
                existingTaskEmployee.TaskId = updateTaskEmployeeDto.TaskId;

            if (updateTaskEmployeeDto.UserId.HasValue)
                existingTaskEmployee.UserId = updateTaskEmployeeDto.UserId;

            if (updateTaskEmployeeDto.AssignedAt.HasValue)
                existingTaskEmployee.AssignedAt = updateTaskEmployeeDto.AssignedAt;

            if (updateTaskEmployeeDto.StatusId.HasValue)
                existingTaskEmployee.StatusId = updateTaskEmployeeDto.StatusId;

            existingTaskEmployee.Deadline = updateTaskEmployeeDto.Deadline;

            if (updateTaskEmployeeDto.CompletionDate.HasValue)
                existingTaskEmployee.CompletionDate = updateTaskEmployeeDto.CompletionDate;

            if (updateTaskEmployeeDto.PenaltyStatus.HasValue)
                existingTaskEmployee.PenaltyStatus = updateTaskEmployeeDto.PenaltyStatus;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
            return existingTaskEmployee;
        }


        // Xóa TaskEmployee
        public async Task DeleteTaskEmployeeAsync(int id)
        {
            var taskEmployee = await _context.TaskEmployees.FindAsync(id);
            if (taskEmployee == null)
            {
                throw new KeyNotFoundException("TaskEmployee not found.");
            }

            _context.TaskEmployees.Remove(taskEmployee);
            await _context.SaveChangesAsync();
        }

        // Lấy TaskEmployee theo ID
        public async Task<TaskEmployee> GetTaskEmployeeByIdAsync(int id)
        {
            var taskEmployee = await _context.TaskEmployees.Include(p=>p.Task).Include(s => s.Status).Include(u => u.User).FirstOrDefaultAsync(f=>f.Id==id);
            if (taskEmployee == null)
            {
                throw new KeyNotFoundException("TaskEmployee not found.");
            }
            return taskEmployee;
        }

        // Lấy danh sách TaskEmployee theo tên công việc
        public async Task<IEnumerable<TaskEmployee>> GetTaskEmployeesByTaskNameAsync(string taskName)
        {
            var taskEmployees = await _context.TaskEmployees
                                               .Include(te => te.Task)
                                               .Where(te => te.Task != null && te.Task.Name.Contains(taskName))
                                               .ToListAsync();
            return taskEmployees;
        }

        // Lấy danh sách TaskEmployee theo tên người dùng
        public async Task<IEnumerable<TaskEmployee>> GetTaskEmployeesByUserNameAsync(string name)
        {
            var taskEmployees = await _context.TaskEmployees
                                               .Include(te => te.User)
                                               .Where(te => te.User != null && te.User.Name.Contains(name))
                                               .ToListAsync();
            return taskEmployees;
        }

   
        public async Task<IEnumerable<TaskEmployeeDto>> GetAllTaskEmployeesAsync()
        {
            return await _context.TaskEmployees
                .Include(te => te.Task)
                .Include(te => te.User)
                .Include(te => te.Status)
                .Select(te => new TaskEmployeeDto
                {
                    Id = te.Id,
                    TaskId = te.TaskId,
                    TaskName = te.Task.Name,
                    UserId = te.UserId,
                    UserName = te.User.Name,
                    StatusId = te.StatusId,
                    Description = te.Status.Description,
                    AssignedAt = te.AssignedAt,
                    Deadline = te.Deadline,
                    CompletionDate = te.CompletionDate,
                    PenaltyStatus = te.PenaltyStatus
                })
                .ToListAsync();
        }
        //Hàm phân trang
        public async Task<PagedResult<TaskEmployeeDto>> GetPagedTaskEmployeesAsync(int pageNumber, int pageSize)
        {
            var query = _context.TaskEmployees
                .Include(te => te.Task)
                .Include(te => te.User)
                .Include(te => te.Status)
                .Select(te => new TaskEmployeeDto
                {
                    Id = te.Id,
                    TaskId = te.TaskId,
                    TaskName = te.Task.Name,
                    UserId = te.UserId,
                    UserName = te.User.Name,
                    StatusId = te.StatusId,
                    Description = te.Status.Description,
                    AssignedAt = te.AssignedAt,
                    Deadline = te.Deadline,
                    CompletionDate = te.CompletionDate,
                    PenaltyStatus = te.PenaltyStatus
                });

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TaskEmployeeDto>
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task UpdateTaskStatusAsync(int taskEmployeeId, int newStatusId)
        {
            // Tìm TaskEmployee theo ID
            var taskEmployee = await _context.TaskEmployees.FindAsync(taskEmployeeId);
            if (taskEmployee == null)
            {
                throw new KeyNotFoundException("TaskEmployee not found.");
            }

            // Cập nhật trạng thái TaskEmployee
            taskEmployee.StatusId = newStatusId;

            // Nếu trạng thái là "Hoàn thành", cập nhật CompletionDate
            if (newStatusId == 3) // Giả sử ID 3 là trạng thái 'Hoàn thành'
            {
                taskEmployee.CompletionDate = DateTime.Now;

                // Tìm Task tương ứng trong bảng ProjectTasks
                var projectTask = await _context.ProjectTasks.FindAsync(taskEmployee.TaskId);
                if (projectTask != null)
                {
                    // Cập nhật trạng thái của Task trong ProjectTasks
                    projectTask.StatusId = 3; // Giả sử ID 3 là trạng thái 'Hoàn thành'
                }
            }

            await _context.SaveChangesAsync();

            // Sau khi cập nhật trạng thái công việc, kiểm tra và cập nhật trạng thái dự án
            var projectId = await _context.ProjectTasks
                                       .Where(pt => pt.Id == taskEmployee.TaskId)
                                       .Select(pt => pt.ProjectId)
                                       .FirstOrDefaultAsync();
           

            if (projectId != 0)
            {
                await CheckAndUpdateProjectStatusAsync(projectId);
            }
        }

        public async Task CheckAndUpdateProjectStatusAsync(int? projectId)
    {
        // Lấy danh sách tất cả công việc trong dự án
        var tasksInProject = await _context.ProjectTasks
                                           .Where(pt => pt.ProjectId == projectId)
                                           .Select(pt => pt.Id)
                                           .ToListAsync();

        // Kiểm tra trạng thái của tất cả công việc
        var incompleteTasks = await _context.TaskEmployees
                                            .Where(te => tasksInProject.Contains(te.TaskId.Value) && te.StatusId !=3)
                                            .AnyAsync();

        if (!incompleteTasks)
        {
            // Nếu không còn công việc nào chưa hoàn thành, cập nhật trạng thái dự án
            var project = await _context.Projects.FindAsync(projectId);
            if (project != null)
            {
                    project.StatusId = 3;
                await _context.SaveChangesAsync();
            }
        }
        }

        public async Task<IEnumerable<TaskEmployee>> GetTaskEmployeeByUserId(int userId)
        {
            if(userId==null)
            {
                throw new KeyNotFoundException("Id Not Found");
            }
            // Lấy danh sách công việc từ database liên kết với userId
            var tasks = await _context.TaskEmployees.Include(p=>p.Task).Include(c=>c.User).Include(f => f.Status).Where(te => te.UserId == userId).ToListAsync();
         
            return tasks;
        }
    }
}
