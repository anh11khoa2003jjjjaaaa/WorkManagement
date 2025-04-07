using Microsoft.AspNetCore.Mvc;
using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskEmployeeController : ControllerBase
    {
        private readonly ITaskEmployeeService _taskEmployeeService;

        public TaskEmployeeController(ITaskEmployeeService taskEmployeeService)
        {
            _taskEmployeeService = taskEmployeeService;
        }

        // GET api/taskemployee
       
        [HttpGet]
        public async Task<IActionResult> GetAllTaskEmployees()
        {
            try
            {
                var taskEmployees = await _taskEmployeeService.GetAllTaskEmployeesAsync();
                return Ok(taskEmployees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving task employees.", error = ex.Message });
            }
        }
        //Hàm phân trang
        [HttpGet("/page")]
        public async Task<IActionResult> GetPagedTaskEmployees([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedResult = await _taskEmployeeService.GetPagedTaskEmployeesAsync(pageNumber, pageSize);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving paged task employees.", error = ex.Message });
            }
        }


        // GET api/taskemployee/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskEmployeeById(int id)
        {
            try
            {
                var taskEmployee = await _taskEmployeeService.GetTaskEmployeeByIdAsync(id);
                return Ok(taskEmployee);  // Trả về TaskEmployee theo ID
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> AddTaskEmployee([FromBody] CreateTaskEmployeeDto createTaskEmployeeDto)
        {
            if (createTaskEmployeeDto == null)
            {
                return BadRequest(new { message = "Invalid input data." });
            }

            // Map từ CreateTaskEmployeeDto sang TaskEmployee
            var taskEmployee = new TaskEmployee
            {
                TaskId = createTaskEmployeeDto.TaskId,
                UserId = createTaskEmployeeDto.UserId,
                StatusId = createTaskEmployeeDto.StatusId,
                AssignedAt = createTaskEmployeeDto.AssignedAt ?? DateTime.UtcNow, // Gán giá trị mặc định nếu null
                Deadline = createTaskEmployeeDto.Deadline,
                CompletionDate = null,  // Mặc định null khi tạo mới
                PenaltyStatus = null    // Mặc định null khi tạo mới
            };

            try
            {
                // Lưu vào cơ sở dữ liệu
                var createdTaskEmployee = await _taskEmployeeService.AddTaskEmployeeAsync(taskEmployee);

                // Trả về dữ liệu đã tạo
                return CreatedAtAction(nameof(GetTaskEmployeeById), new { id = createdTaskEmployee.Id }, createdTaskEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the task employee.", error = ex.Message });
            }
        }

        // PUT api/taskemployee/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskEmployee(int id, [FromBody] UpdateTaskEmployeeDto updateTaskEmployeeDto)
        {
            if (updateTaskEmployeeDto == null)
            {
                return BadRequest(new { message = "Invalid input data." });
            }

            try
            {
                // Gọi service để cập nhật
                var updatedTaskEmployee = await _taskEmployeeService.UpdateTaskEmployeeAsync(id, updateTaskEmployeeDto);
                return Ok(updatedTaskEmployee);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task employee.", error = ex.Message });
            }
        }


        // DELETE api/taskemployee/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskEmployee(int id)
        {
            try
            {
                await _taskEmployeeService.DeleteTaskEmployeeAsync(id);
                return NoContent();  // Trả về HTTP 204 No Content nếu xóa thành công
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the task employee.", error = ex.Message });
            }
        }
        // PUT api/taskemployee/updatestatus/{taskEmployeeId}
        [HttpPut("updatestatus/{taskEmployeeId}")]
        public async Task<IActionResult> UpdateTaskStatus(int taskEmployeeId, [FromBody] int newStatusId)
        {
            try
            {
                await _taskEmployeeService.UpdateTaskStatusAsync(taskEmployeeId, newStatusId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task status.", error = ex.Message });
            }
        }

        // POST api/taskemployee/updateprojectstatus/{projectId}
        [HttpPost("updateprojectstatus/{projectId}")]
        public async Task<IActionResult> CheckAndUpdateProjectStatus(int projectId)
        {
            try
            {
                await _taskEmployeeService.CheckAndUpdateProjectStatusAsync(projectId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the project status.", error = ex.Message });
            }
        }


        [HttpGet("taskByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskEmployee>>> GetTasksByUserId(int userId)
        {
            try
            {
                var tasks = await _taskEmployeeService.GetTaskEmployeeByUserId(userId);

                if (tasks == null || !tasks.Any())
                {
                    return NotFound(new { message = "Không có công việc nào liên kết với userId này." });
                }

                return Ok(tasks); // Trả về danh sách công việc
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi.", details = ex.Message });
            }
        }
    }
}
