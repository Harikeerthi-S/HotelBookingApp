using HotelBookingApp.Models.Dtos;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            try
            {
                var room = await _roomService.CreateRoomAsync(dto);
                return CreatedAtAction(nameof(GetById), new { roomId = room.RoomId }, room);
            }
            catch (ArgumentException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error creating room", Details = ex.Message }); }
        }

        [HttpGet("{roomId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int roomId)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(roomId);
                if (room == null) return NotFound(new { Message = "Room not found." });
                return Ok(room);
            }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error retrieving room", Details = ex.Message }); }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] int? hotelId)
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync(hotelId);
                return Ok(rooms);
            }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error retrieving rooms", Details = ex.Message }); }
        }

        [HttpPut("{roomId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int roomId, [FromBody] CreateRoomDto dto)
        {
            try
            {
                var room = await _roomService.UpdateRoomAsync(roomId, dto);
                if (room == null) return NotFound(new { Message = "Room not found." });
                return Ok(room);
            }
            catch (ArgumentException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error updating room", Details = ex.Message }); }
        }

        [HttpDelete("{roomId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Deactivate(int roomId)
        {
            try
            {
                var result = await _roomService.DeactivateRoomAsync(roomId);
                if (!result) return NotFound(new { Message = "Room not found." });
                return NoContent();
            }
            catch (InvalidOperationException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error deactivating room", Details = ex.Message }); }
        }

        [HttpPost("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> Filter([FromBody] RoomFilterDto filter)
        {
            try
            {
                var rooms = await _roomService.GetRoomsFilteredAsync(filter);
                return Ok(rooms);
            }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error filtering rooms", Details = ex.Message }); }
        }

        [HttpPost("filter/paged")]
        [AllowAnonymous]
        public async Task<IActionResult> FilterPaged([FromBody] RoomFilterDto filter, [FromQuery] PagedRequestDto pageRequest)
        {
            try
            {
                var pagedRooms = await _roomService.GetRoomsFilteredPagedAsync(filter, pageRequest);
                return Ok(pagedRooms);
            }
            catch (Exception ex) { return StatusCode(500, new { Message = "Error filtering rooms with pagination", Details = ex.Message }); }
        }
    }
}