using Microsoft.AspNetCore.Mvc;
using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusJobController : ControllerBase
    {
        private readonly IStatusJobService _statusJobService;

        public StatusJobController(IStatusJobService statusJobService)
        {
            _statusJobService = statusJobService;
        }

        // GET api/statusjob
        [HttpGet]
        public async Task<IActionResult> GetAllProjectStatuses()
        {
            try
            {
                var statuses = await _statusJobService.GetAllProjectStatusesAsync();
                return Ok(statuses);  // Trả về danh sách tất cả trạng thái công việc
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving project statuses.", error = ex.Message });
            }
        }

        // GET api/statusjob/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectStatusById(int id)
        {
            try
            {
                var status = await _statusJobService.GetProjectStatusByIdAsync(id);
                return Ok(status);  // Trả về trạng thái công việc theo ID
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST api/statusjob
        [HttpPost]
        public async Task<IActionResult> AddProjectStatus([FromBody] StatusJobDto statusJobDto)
        {
            var statusJob = new StatusJob
            {
                Name = statusJobDto.Name,
                Description = statusJobDto.Description
            };

            try
            {
                var createdStatus = await _statusJobService.AddProjectStatusAsync(statusJob);
                return CreatedAtAction(nameof(GetProjectStatusById), new { id = createdStatus.Id }, createdStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the project status.", error = ex.Message });
            }
        }

        // PUT api/statusjob/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProjectStatus(int id, [FromBody] StatusJobDto statusJobDto)
        {
            var statusJob = new StatusJob
            {
                Name = statusJobDto.Name,
                Description = statusJobDto.Description
            };

            try
            {
                var updatedStatus = await _statusJobService.UpdateProjectStatusAsync(id, statusJob);
                return Ok(updatedStatus);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the project status.", error = ex.Message });
            }
        }

        // DELETE api/statusjob/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectStatus(int id)
        {
            try
            {
                await _statusJobService.DeleteProjectStatusAsync(id);
                return NoContent();  // Trả về HTTP 204 No Content nếu xóa thành công
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the project status.", error = ex.Message });
            }
        }
    }
}
