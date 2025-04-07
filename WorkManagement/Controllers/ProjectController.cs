using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddProject([FromForm] ProjectDto projectDto, [FromForm] List<IFormFile> images)
        {
            if (projectDto == null)
            {
                return BadRequest("Invalid project data.");
            }

            try
            {
                // Gọi service để thêm dự án
                var createdProject = await _projectService.AddProjectAsync(projectDto, images);

                // Trả về kết quả sau khi tạo dự án
                return CreatedAtAction(nameof(GetProjectById), new { id = createdProject.Id }, createdProject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// Cập nhật một dự án cùng với hình ảnh liên quan.
        /// </summary>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromForm] ProjectDto projectDto, [FromForm] List<IFormFile> images)
        {
            if (projectDto == null)
            {
                return BadRequest("Invalid project data.");
            }

            try
            {
                var updatedProject = await _projectService.UpdateProjectAsync(id, projectDto, images);
                return Ok(updatedProject);
            }
            catch (DbUpdateException ex)
            {
                // Ghi log lỗi chi tiết
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw; // Để trả lỗi về client nếu cần
            }
        }

        /// <summary>
        /// Xóa một dự án bằng ID.
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                return Ok("Project deleted successfully.");
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

        /// <summary>
        /// Lấy thông tin một dự án bằng ID, bao gồm hình ảnh liên quan.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);

                if(project == null)
                {
                    return NotFound();
                }

                var ProjectWithImage = new
                {
                    project.Id,
                    project.Name,
                    project.Description,
                    project.StartDate,
                    project.EndDate,
                    project.ManagerId,
                    project.StatusId,
                    project.DepartmentId,
                    Images = project.Images.Select(img => img.FilePath).ToList()

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

        /// <summary>
        /// Lấy danh sách tất cả các dự án, bao gồm hình ảnh liên quan.
        /// </summary>
        [HttpGet("getAllPage")]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();

                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllProjects(int page = 1, int pageSize = 10)
        {
            try
            {
                var projects = await _projectService.GetAllPageProjectsAsync(page, pageSize);

                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetProjectsByDepartment(int departmentId)
        {
            try
            {
                var projects = await _projectService.GetProjectsByDepartmentIdAsync(departmentId);
                return Ok(projects);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", detail = ex.Message });
            }
        }

    }
}
