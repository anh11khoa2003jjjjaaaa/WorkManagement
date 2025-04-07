using Microsoft.AspNetCore.Mvc;
using WorkManagement.Interfaces;
using WorkManagement.Models;
using WorkManagement.DTO;
using Org.BouncyCastle.Asn1.Crmf;
using WorkManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace WorkManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTaskController : ControllerBase
    {
        private readonly IProjectTaskService _projectTaskService;

        public ProjectTaskController(IProjectTaskService projectTaskService)
        {
            _projectTaskService = projectTaskService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProjectTasks()
        {
            try
            {
                var projectTasks = await _projectTaskService.GetAll();
                return Ok(projectTasks);  // Trả về danh sách ProjectTask
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the project tasks.", error = ex.Message });
            }
        }
        //Phân trang

        [HttpGet("paged")]
        public async Task<IActionResult> GetAllProjectTasksPaged(int page = 1, int pageSize = 10)
        {
            try
            {
                var pagedResult = await _projectTaskService.GetAllPaged(page, pageSize);
                return Ok(pagedResult); // Trả về danh sách ProjectTask phân trang
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the project tasks.", error = ex.Message });
            }
        }


        // Thêm ProjectTask
        [HttpPost]

        public async Task<IActionResult> AddProjectTask([FromForm] ProjectTaskDto projectTaskDto, [FromForm] List<IFormFile> images)
        {
            if (projectTaskDto == null)
            {
                return BadRequest("Invalid project data.");
            }

            try
            {
                // Gọi service để thêm dự án
                var createdProject = await _projectTaskService.AddProjectTaskAsync(projectTaskDto, images);

                // Trả về kết quả sau khi tạo dự án
                return CreatedAtAction(nameof(GetProjectTaskById), new { id = createdProject.Id }, createdProject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       

        //update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProjectTask(int id, [FromForm] ProjectTaskDto projectTaskDto, [FromForm] List<IFormFile> images)
        {
            var projectTask = new ProjectTask
            {
                Name = projectTaskDto.Name,
                Description = projectTaskDto.Description,
                ProjectId = projectTaskDto.ProjectId,
                StatusId = projectTaskDto.StatusId
            };

            try
            {
                var updatedTask = await _projectTaskService.UpdateProjectTaskAsync(id, projectTaskDto, images);
                return Ok(updatedTask);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        

            // Xóa ProjectTask
            [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectTask(int id)
        {
            try
            {
                await _projectTaskService.DeleteProjectTaskAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
                [HttpGet("{id}")]
                public async Task<IActionResult> GetProjectTaskById(int id)
                {
                    try
                    {
                        var projecttask = await _projectTaskService.GetProjectTaskByIdAsync(id);

                        if (projecttask == null)
                        {
                            return NotFound();
                        }

                        var ProjectWithImage = new
                        {
                            projecttask.Id,
                            projecttask.Name,
                            projecttask.Description,
                            projecttask.ProjectId,
                            


                            projecttask.StatusId,
                           
                            Images = projecttask.Images.Select(img => img.FilePath).ToList()

                        };
                        return Ok(ProjectWithImage);


                    }
                    catch (KeyNotFoundException ex)
                    {
                        return NotFound(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }

                // Lấy ProjectTask theo tên
                [HttpGet("search")]
        public async Task<IActionResult> GetProjectTasksByName([FromQuery] string name)
        {
            var tasks = await _projectTaskService.GetProjectTasksByNameAsync(name);
            return Ok(tasks);
        }

        [HttpGet("taskByProject/{projectId}")]
        public async Task<IActionResult> GetTasksByProjectId(int projectId)
        {
            try
            {
                var tasks = await _projectTaskService.GetTasksByProjectIdAsync(projectId);
                return Ok(tasks); // Trả về danh sách tasks
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        // API Get Available Tasks (Lấy các công việc chưa được phân công cho nhân viên)
        [HttpGet("available-tasks")]
        public async Task<IActionResult> GetAvailableTasksAsync()
        {
            try
            {
                var availableTasks = await _projectTaskService.GetUnassignedTasksAsync();

                if (availableTasks.Count == 0)
                {
                    return NotFound("No available tasks found for the given employee.");
                }

                return Ok(availableTasks);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
