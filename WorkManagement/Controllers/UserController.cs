using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Models;
using WorkManagement.Service;

namespace WorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Thêm mới người dùng
        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromForm] UserDto userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Phone = userDto.Phone,
                BirthDay = userDto.BirthDay,
                DepartmentId = userDto.DepartmentId,
                PositionId = userDto.PositionId,
                IsLeader = false,
            };

            var createdUser = await _userService.AddUserAsync(user, userDto.Avatar);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        // Cập nhật người dùng
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromForm] UserDto userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Phone = userDto.Phone,
                BirthDay = userDto.BirthDay,
                DepartmentId = userDto.DepartmentId,
                PositionId = userDto.PositionId,
                IsLeader = userDto.isLeader,
            };

            var updatedUser = await _userService.UpdateUserAsync(id, user, userDto.Avatar);
            return Ok(updatedUser);
        }

        // Xóa người dùng
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        // Lấy thông tin người dùng theo ID
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        // Tìm kiếm người dùng theo tên
        [HttpGet("search")]
        public async Task<IActionResult> GetUsersByName([FromQuery] string name)
        {
            var users = await _userService.GetUsersByNameAsync(name);
            return Ok(users);
        }

        [HttpPut("update-avatar/{id}")]
        [Consumes("multipart/form-data")] // Bắt buộc chỉ định kiểu dữ liệu
        public async Task<IActionResult> UpdateAvatar(int id, [FromForm] IFormFile avatarFile)
        {
            if (avatarFile == null || avatarFile.Length == 0)
                return BadRequest(new { message = "Please provide a valid avatar file." });

            try
            {
                var updatedUser = await _userService.UpdateAvatarAsync(id, avatarFile);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("update-department/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromForm] int departmentId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _userService.UpdateDepartmentAsync(id, departmentId);
                return Ok(new { message = "Cập nhật phòng ban thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("leaders")]
        public async Task<IActionResult> GetLeaders()
        {
            var leaders = await _userService.GetUserByIsLeader();
            if (!leaders.Any())
                return NotFound("Không có trưởng phòng nào.");

            return Ok(leaders);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            if (!users.Any())
                return NotFound("Không có user nào.");

            return Ok(users);
        }
        //Phân trang
        [HttpGet("paged")]
        public async Task<IActionResult> GetAllPaged(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page và PageSize phải lớn hơn 0.");

            var pagedResult = await _userService.GetAllPaged(page, pageSize);

            if (pagedResult == null || (pagedResult as dynamic).TotalCount == 0)
                return NotFound("Không có user nào.");

            return Ok(pagedResult);
        }


        [HttpGet("by-task/{taskId}")]
        public async Task<IActionResult> GetEmployeesByTask(int taskId)
        {
            try
            {
                var employees = await _userService.GetEmployeesForTaskAsync(taskId);
                if (employees == null || employees.Count == 0)
                {
                    return NotFound(new { Message = "Không tìm thấy nhân viên nào cho công việc này." });
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, new { Message = "Đã xảy ra lỗi trong quá trình xử lý.", Error = ex.Message });
            }
        }

        [HttpGet("getByAccountId/{accountId}")]
        public async Task<IActionResult> GetUserByAccountId(int accountId)
        {
            var user = await _userService.GetUserbyAccountId(accountId);
            if (user == null)
            {
                return NotFound(new { message = $"Không tìm thấy User cho AccountId {accountId}." });
            }
            return Ok(user);
        }
        //Thống kê nhân viên thuộc phòng ban
        [HttpGet("statistical-employees")]
        public async Task<IActionResult> GetStatisticalEmployeesOfDepartment()
        {
            var result = await _userService.StatisticalEmployeeOfDepartment();
            if (result == null || !result.Any())
            {
                return NotFound(new { Message = "No data found." });
            }

            return Ok(result);
        }
        //Thống kê có bao nhiêu dự án hoàn thành và chưa
        [HttpGet("project-status-statistics")]
        public async Task<IActionResult> GetProjectStatusStatistics()
        {
            var result = await _userService.GetProjectStatusStatistics();
            return Ok(result);
        }
        //Thống kê có bao nhiêu công việc đã hoàn thành
        [HttpGet("employee-task-completion-status")]
        public async Task<IActionResult> GetEmployeeTaskCompletionStatus()
        {
            var result = await _userService.GetEmployeeTaskCompletionStatusWithCount();
            return Ok(result);
        }
        //Thống kê danh sách công việc theo 1 dụ án
        [HttpGet("project-task-counts")]
        public async Task<IActionResult> GetProjectTaskCounts()
        {
            var projectTaskCounts = await _userService.GetProjectTaskCountsAsync();

            if (projectTaskCounts == null)
            {
                return NotFound();
            }

            return Ok(projectTaskCounts);  // Trả về kiểu object
        }
        

    }
}
