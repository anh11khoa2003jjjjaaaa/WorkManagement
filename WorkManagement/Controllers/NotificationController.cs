using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WorkManagement.Services;

namespace WorkManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        /// <summary>
        /// Constructor để inject NotificationService.
        /// </summary>
        /// <param name="notificationService">Service xử lý thông báo</param>
        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Gửi thông báo đến người dùng hoặc toàn bộ nhân viên.
        /// </summary>
        /// <param name="userId">ID người nhận (null nếu gửi toàn bộ)</param>
        /// <param name="title">Tiêu đề thông báo</param>
        /// <param name="content">Nội dung thông báo</param>
        /// <param name="isGlobal">Có phải thông báo toàn bộ hay không</param>
        /// <returns>Kết quả gửi thông báo</returns>
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification(
            [FromQuery] int? userId,
            [FromQuery] string title,
            [FromQuery] string content,
            [FromQuery] bool isGlobal = false)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Tiêu đề và nội dung thông báo không được để trống.");
            }

            try
            {
                await _notificationService.SendNotificationAsync(userId, title, content, isGlobal);
                return Ok(new { message = "Thông báo đã được gửi thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi gửi thông báo.", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách thông báo của người dùng.
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        /// <returns>Danh sách thông báo</returns>
        [HttpGet("get-notifications")]
        public async Task<IActionResult> GetNotifications([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }

            try
            {
                var result = await _notificationService.GetNotificationsAsync(userId);
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách thông báo.", error = ex.Message });
            }
        }



        [HttpPatch("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID thông báo không hợp lệ.");
            }

            try
            {
                var result = await _notificationService.MarkAsReadAsync(id);
                if (result)
                {
                    return Ok(new { message = "Thông báo đã được đánh dấu là đã đọc." });
                }
                else
                {
                    return NotFound("Không tìm thấy thông báo.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật trạng thái thông báo.", error = ex.Message });
            }
        }

    }
}
