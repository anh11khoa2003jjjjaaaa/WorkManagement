//using Microsoft.AspNetCore.Mvc;
//using WorkManagement.Interfaces;
//using WorkManagement.Models;

//namespace WorkManagement.Controllers
//{

//    [ApiController]
//    [Route("api/[controller]")]
//    public class PositionController : ControllerBase
//    {
//        private readonly IPositionService _positionService;

//        public PositionController(IPositionService positionService)
//        {
//            _positionService = positionService;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Position>>> GetAll()
//        {
//            var positions = await _positionService.GetAll();
//            return Ok(positions);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<Position>> GetById(int id)
//        {
//            var position = await _positionService.GetById(id);
//            if (position == null)
//            {
//                return NotFound();
//            }
//            return Ok(position);
//        }

//        [HttpPost]
//        public async Task<ActionResult> Add(Position position)
//        {
//            await _positionService.Add(position);
//            return CreatedAtAction(nameof(GetById), new { id = position.Id }, position);
//        }

//        [HttpPut("{id}")]
//        public async Task<ActionResult> Update(int id, [FromBody] Position position)
//        {
//            if (id != position.Id)
//            {
//                return BadRequest();
//            }

//            var existingPosition = await _positionService.GetById(id);
//            if (existingPosition == null)
//            {
//                return NotFound();
//            }

//            await _positionService.Update(position);
//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(int id)
//        {
//            var position = await _positionService.GetById(id);
//            if (position == null)
//            {
//                return NotFound();
//            }

//            await _positionService.Delete(id);
//            return NoContent();
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Position>>> GetAll()
        {
            var positions = await _positionService.GetAll();
            return Ok(positions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Position>> GetById(int id)
        {
            var position = await _positionService.GetById(id);
            if (position == null)
            {
                return NotFound();
            }
            return Ok(position);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] Position position)
        {
            if (position == null || string.IsNullOrEmpty(position.Name))
            {
                return BadRequest("Position data is invalid. Name is required.");
            }

            try
            {
                await _positionService.Add(position);
                return CreatedAtAction(nameof(GetById), new { id = position.Id }, position);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Position position)
        {
            if (position == null || string.IsNullOrEmpty(position.Name))
            {
                return BadRequest("Position data is invalid. Name is required.");
            }

           

            var existingPosition = await _positionService.GetById(id);
            if (existingPosition == null)
            {
                return NotFound("Position not found.");
            }

            try
            {
                await _positionService.Update(id,position);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var position = await _positionService.GetById(id);
            if (position == null)
            {
                return NotFound("Position not found.");
            }

            try
            {
                await _positionService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
