using Microsoft.AspNetCore.Mvc;
using study_buddys_backend_v2.Models;
using Microsoft.AspNetCore.SignalR;
using study_buddys_backend_v2.Hubs;
using study_buddys_backend_v2.Context;
using Microsoft.EntityFrameworkCore;

namespace study_buddys_backend_v2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DirectMessagesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IHubContext<DirectMessageHub> _hubContext;

        public DirectMessagesController(DataContext context, IHubContext<DirectMessageHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/DirectMessages/{id}
        [HttpGet("getAllUsersChats/{id}")]
        public async Task<ActionResult<DirectMessageModel>> getAllUsersChats(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var messages = await _context.DirectMessages.Where(m => m.SenderId == id || m.ReceiverId == id).ToListAsync();

            if (messages == null || !messages.Any())
            {
                return NotFound(new { Success = false, Message = "No messages found for this user." });
            }
            
            var groupedChats = messages.GroupBy(m => m.ReceiverId == id ? m.SenderId : m.ReceiverId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Messages = g.OrderBy(m => m.DateTime).ToList()
                }).ToList();
            return Ok(new { Success = true, Chats = groupedChats });
        }



        // POST: api/DirectMessages
        [HttpPost("PostDirectMessage")]
        public async Task<ActionResult<DirectMessageModel>> PostDirectMessage(DirectMessageModel message)
        {
            if (message == null)
            {
                return BadRequest("Message cannot be null.");
            }

            // Save the message to the database
            _context.DirectMessages.Add(message);
            await _context.SaveChangesAsync();

            // Send message via SignalR to the receiver (real-time update)
            await _hubContext.Clients.User(message.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", message.SenderId.ToString(), message.Message);

            // Return a 200 OK with the message
            return Ok(new { Success = true, Message = message });
        }


    }
}
