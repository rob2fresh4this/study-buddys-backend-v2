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
        public async Task<ActionResult> getAllUsersChats(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Success = false, Message = "Invalid user ID." });
            }

            // Retrieve all messages, including deleted ones
            var messages = await _context.DirectMessages
                .Where(m => m.SenderId == id || m.ReceiverId == id)
                .ToListAsync();

            if (messages == null || !messages.Any())
            {
                return NotFound(new { Success = false, Message = "No messages found for this user." });
            }

            // Group messages by the other user (ReceiverId or SenderId)
            var groupedChats = messages
                .GroupBy(m => m.ReceiverId == id ? m.SenderId : m.ReceiverId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Messages = g.OrderBy(m => m.DateTime).Select(msg => new
                    {
                        msg.Id,
                        msg.SenderId,
                        msg.ReceiverId,
                        // If the message is deleted, replace its content with the deletion notice
                        Message = msg.IsDeleted ? $"Message was deleted at: {msg.DeletedAt}" : msg.Message,
                        msg.AttachmentUrl,
                        msg.DateTime,
                        msg.IsRead, // Ensure you're using the correct read status for the user
                        msg.RecieverRead, // Ensure this is the correct field to track if the receiver read the message
                        msg.IsEdited,
                        msg.EditedAt
                    }).ToList()
                })
                .ToList();

            return Ok(new { Success = true, Chats = groupedChats });
        }



        // POST: api/DirectMessages
        [HttpPost("PostDirectMessage")]
        public async Task<ActionResult> PostDirectMessage(DirectMessageModel message)
        {
            if (message == null)
            {
                return BadRequest(new { Success = false, Message = "Message cannot be null." });
            }

            _context.DirectMessages.Add(message);
            await _context.SaveChangesAsync();

            // Send SignalR message
            await _hubContext.Clients.User(message.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", message.SenderId.ToString(), message.Message);

            // Return a cleaner version of the posted message
            var responseMessage = new
            {
                message.Id,
                message.SenderId,
                message.ReceiverId,
                message.Message,
                message.AttachmentUrl,
                message.DateTime,
                message.IsRead,
                message.IsEdited,
                message.EditedAt
            };

            return Ok(new { Success = true, Message = responseMessage });
        }

        [HttpPut("EditMessage/{idOfMessage}")]
        public async Task<ActionResult> EditMessage(int idOfMessage, [FromBody] string newMessage)
        {
            // Find the message by ID
            var message = await _context.DirectMessages.FindAsync(idOfMessage);

            // If the message doesn't exist, return a NotFound error
            if (message == null)
            {
                return NotFound(new { Success = false, Message = "Message not found." });
            }

            // Check if the message is marked as deleted, prevent editing if it is
            if (message.IsDeleted)
            {
                return BadRequest(new { Success = false, Message = "Cannot edit a deleted message." });
            }

            // Update the message with the new content
            message.Message = newMessage;

            // Mark the message as edited and set the edited timestamp
            message.IsEdited = true;
            message.EditedAt = DateTime.UtcNow;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return a success response
            return Ok(new { Success = true, Message = "Message updated successfully." });
        }

        [HttpDelete("softDeleteMessage/{messageId}")]
        public async Task<ActionResult> SoftDeleteMessage(int messageId)
        {
            var message = await _context.DirectMessages
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
            {
                return NotFound(new { Success = false, Message = "Message not found." });
            }

            // Set IsDeleted to true and set the DeletedAt timestamp
            message.IsDeleted = true;
            message.DeletedAt = DateTime.UtcNow;

            _context.DirectMessages.Update(message);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Message soft deleted successfully." });
        }

        [HttpGet("MarkAsRead/{messageId}/{senderBool}/{receiverBool}")]
        public async Task<ActionResult> MarkAsRead(int messageId, bool senderBool, bool receiverBool)
        {
            var message = await _context.DirectMessages.FindAsync(messageId);

            if (message == null)
            {
                return NotFound(new { Success = false, Message = "Message not found." });
            }

            // Update the read status based on the parameters
            if (senderBool)
            {
                message.IsRead = true;
            }
            if (receiverBool)
            {
                message.RecieverRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Message marked as read." });
        }





    }
}
