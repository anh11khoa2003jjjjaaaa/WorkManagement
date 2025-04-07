using Microsoft.AspNetCore.Mvc;
using WorkManagement.Interfaces;
using WorkManagement.Models;
using WorkManagement.Service;

namespace WorkManagement.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
        }

        /// <summary>
        /// Thêm hình ảnh mới.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> AddImage([FromBody] Image image)
        {
            if (image == null)
            {
                return BadRequest("Invalid image data.");
            }

            await _imageService.AddImageAsync(image);
            return Ok("Image added successfully.");
        }

        /// <summary>
        /// Xóa hình ảnh theo ID.
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> RemoveImage(int id)
        {
            try
            {
                await _imageService.RemoveImageAsync(id);
                return Ok("Image removed successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lấy thông tin hình ảnh theo ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var image = await _imageService.GetImageByIdAsync(id);

            if (image == null)
            {
                return NotFound($"No image found with ID {id}.");
            }

            return Ok(image);
        }

        /// <summary>
        /// Lấy danh sách hình ảnh theo ID dự án.
        /// </summary>
        [HttpGet("getByProjectId/{projectId}")]
        public async Task<ActionResult<IEnumerable<Image>>> GetImagesByProjectIdAsync(int projectId)
        {
            var images = await _imageService.GetImagesByProjectIdAsync(projectId);

            if (images == null || !images.Any())
            {
                return NotFound("No images found for this project.");
            }

            return Ok(images);
        }

        /// <summary>
        /// Lấy danh sách hình ảnh theo ID công việc.
        /// </summary>
        [HttpGet("getByTaskId/{taskId}")]
        public async Task<ActionResult<IEnumerable<Image>>> GetImagesByTaskIdAsync(int taskId)
        {
            var images = await _imageService.GetImagesByTaskIdAsync(taskId);

            if (images == null || !images.Any())
            {
                return NotFound("No images found for this task.");
            }

            return Ok(images);
        }
    }
}