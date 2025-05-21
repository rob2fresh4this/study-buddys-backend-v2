using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using study_buddys_backend_v2.Models;
using study_buddys_backend_v2.Services;

namespace study_buddys_backend_v2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommunityEventsController : ControllerBase
    {
        private readonly CommunityEventService _eventService;

        public CommunityEventsController(CommunityEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("getAllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            if (events == null || !events.Any())
                return NotFound(new { Success = false, Message = "No events found" });
            return Ok(new { Success = true, Events = events });
        }

        [HttpGet("getEventsByCommunityId/{communityId}")]
        public async Task<IActionResult> GetEventsByCommunityId(int communityId)
        {
            var events = await _eventService.GetEventsByCommunityIdAsync(communityId);
            if (events == null || !events.Any())
                return NotFound(new { Success = false, Message = "No events found for this community" });
            return Ok(new { Success = true, Events = events });
        }

        [Authorize]
        [HttpPost("createEvent")]
        public async Task<IActionResult> CreateEvent([FromBody] CommunityEventsModel eventModel)
        {
            var created = await _eventService.CreateEventAsync(eventModel);
            if (created == null)
                return BadRequest(new { Success = false, Message = "Failed to create event" });
            return Ok(new { Success = true, Event = created });
        }

        [Authorize]
        [HttpPut("updateEvent")]
        public async Task<IActionResult> UpdateEvent([FromBody] CommunityEventsModel eventModel)
        {
            var updated = await _eventService.UpdateEventAsync(eventModel);
            if (updated == null)
                return NotFound(new { Success = false, Message = "Event not found" });
            return Ok(new { Success = true, Event = updated });
        }

        [Authorize]
        [HttpPost("addParticipant/{eventId}/{userId}")]
        public async Task<IActionResult> AddParticipant(int eventId, int userId)
        {
            var success = await _eventService.AddParticipantsAsync(eventId, userId);
            if (success)
            {
                return Ok(new { Success = true });
            }
            return BadRequest(new { Success = false, message = "Failed to add participant or participant already exists." });
        }

        [Authorize]
        [HttpDelete("deleteEvent/{communityId}/{eventId}")]
        public async Task<IActionResult> DeleteEvent(int communityId, int eventId)
        {
            var deleted = await _eventService.DeleteEventAsync(communityId, eventId, true);
            if (!deleted)
                return BadRequest(new { Success = false, Message = "Failed to delete event" });
            return Ok(new { Success = true });
        }
    }
}
