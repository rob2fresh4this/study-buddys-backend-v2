using Microsoft.AspNetCore.Mvc;
using study_buddys_backend_v2.Models;
using study_buddys_backend_v2.Services;

namespace study_buddys_backend_v2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommunityController : ControllerBase
    {
        private readonly CommunityService _communityServices;

        public CommunityController(CommunityService communityServices)
        {
            _communityServices = communityServices;
        }

        [HttpGet("getAllCommunities")]
        public async Task<IActionResult> GetAllCommunities()
        {
            var communities = await _communityServices.GetAllCommunitiesAsync();
            if (communities == null || !communities.Any())
            {
                return BadRequest(new { Success = false, Message = "No communities found or error retrieving data" });
            }
            return Ok(new { Success = true, Communities = communities });
        }

        [HttpPost("addCommunity")]
        public async Task<IActionResult> AddCommunity([FromBody] CommunityModel community)
        {
            if (await _communityServices.AddCommunityAsync(community)) return Ok(new { Success = true });
            return BadRequest(new { Success = false });
        }

        [HttpPut("updateCommunity")]
        public async Task<IActionResult> UpdateCommunity([FromBody] CommunityModel updatedCommunity)
        {
            var existingCommunity = await _communityServices.GetCommunityByIdAsync(updatedCommunity.Id);
            if (existingCommunity == null)
            {
                return NotFound(new { Success = false, Message = "Community not found" });
            }

            // Preserve existing members and requests
            updatedCommunity.CommunityMembers = existingCommunity.CommunityMembers;
            updatedCommunity.CommunityRequests = existingCommunity.CommunityRequests;

            if (await _communityServices.UpdateCommunityAsync(updatedCommunity))
            {
                return Ok(new { Success = true });
            }

            return BadRequest(new { Success = false, Message = "Failed to update community" });
        }


        [HttpPost("addMemberToCommunity/{communityId}/{userId}")]
        public async Task<IActionResult> AddMemberToCommunity(int communityId, int userId)
        {
            if (await _communityServices.AddMemberToCommunityAsync(communityId, userId)) return Ok(new { Success = true });
            return BadRequest(new { Success = false, message = "Failed to add member to community" });
        }

        [HttpDelete("removeMemberFromCommunity/{communityId}/{userId}")]
        public async Task<IActionResult> RemoveMemberFromCommunity(int communityId, int userId)
        {
            if (await _communityServices.RemoveMemberFromCommunityAsync(communityId, userId)) return Ok(new { Success = true });
            return BadRequest(new { Success = false, message = "Failed to remove member from community" });
        }

        [HttpPost("addRequestToCommunity/{communityId}/{userId}")]
        public async Task<IActionResult> AddRequestToCommunity(int communityId, int userId)
        {
            if (await _communityServices.AddRequestToCommunityAsync(communityId, userId)) return Ok(new { Success = true });
            return BadRequest(new { Success = false, message = "Failed to add request to community" });
        }

        [HttpDelete("removeRequestFromCommunity/{communityId}/{userId}")]
        public async Task<IActionResult> RemoveRequestFromCommunity(int communityId, int userId)
        {
            if (await _communityServices.RemoveRequestFromCommunityAsync(communityId, userId)) return Ok(new { Success = true });
            return BadRequest(new { Success = false, message = "Failed to remove request from community" });
        }

        [HttpPut("approveRequest/{communityId}/{userId}/{approveORnot}")]
        public async Task<IActionResult> ApproveRequest(int communityId, int userId, bool approveORnot)
        {
            if (await _communityServices.ApproveRequestAsync(communityId, userId, approveORnot))
            {
                return Ok(new { Success = true, Message = approveORnot ? "Request approved" : "Request denied" });
            }
            return BadRequest(new { Success = false, message = "Failed to approve request" });
        }



        // only for testing purposes
        [HttpDelete("clearCommunityMembers/{communityId}")]
        public async Task<IActionResult> ClearCommunityMembers(int communityId)
        {
            if (await _communityServices.ClearCommunityMembersAsync(communityId))
                return Ok(new { Success = true });

            return BadRequest(new { Success = false, message = "Failed to clear community members" });
        }

        [HttpPut("EditCommunityRole/{communityId}/{userId}/{role}")]
        public async Task<IActionResult> EditCommunityRole(int communityId, int userId, string role)
        {
            if (await _communityServices.EditCommunityMemberRoleAsync(communityId, userId, role))
                return Ok(new { Success = true });

            return BadRequest(new { Success = false, message = "Failed to edit community role" });
        }

        [HttpGet("FilterUserIdFromCommunityAsync/{userId}")]

        public async Task<IActionResult> FilterUserIdFromCommunityAsync(int userId)
        {
            var communities = await _communityServices.GetCommunitiesByUserIdAsync(userId);

            if (communities == null || communities.Count == 0)
            {
                return NotFound($"No communities found for user with ID {userId}");
            }

            return Ok(communities);
        }

        [HttpPost("CreateCommunityChats")]
        public async Task<IActionResult> CreateCommunityChats(int communityId, [FromBody] CommunityChatModel chat)
        {
            if (await _communityServices.CreateCommunityChatAsync(communityId, chat))
            {
                // Fetch the community again to check if the chat exists
                var community = await _communityServices.GetCommunityByIdAsync(communityId);
                Console.WriteLine($"Community has {community?.CommunityChats.Count} chats.");
                return Ok(new { Success = true });
            }

            return BadRequest(new { Success = false, message = "Failed to create community chat" });
        }

        [HttpPost("EditCommunityChat/{communityId}/{chatId}/{newMessage}")]
        public async Task<IActionResult> EditCommunityChat(int communityId, int chatId, string newMessage)
        {
            if (await _communityServices.EditCommunityChatAsync(communityId, chatId, newMessage))
            {
                return Ok(new { Success = true });
            }

            return BadRequest(new { Success = false, message = "Failed to edit community chat" });
        }

        [HttpPut("PinCommunityChat/{communityId}/{chatId}/{isPinned}")]
        public async Task<IActionResult> PinCommunityChat(int communityId, int chatId, bool isPinned)
        {
            if (await _communityServices.PinCommunityPostAsync(communityId, chatId, isPinned))
            {
                return Ok(new { Success = true });
            }

            return BadRequest(new { Success = false, message = "Failed to pin community chat" });
        }

        [HttpDelete("DeleteCommunityPost/{communityId}/{chatId}/{isDeleted}")]
        public async Task<IActionResult> DeleteCommunityPost(int communityId, int chatId, bool isDeleted)
        {
            if (await _communityServices.DeleteCommunityPostAsync(communityId, chatId, isDeleted))
            {
                return Ok(new { Success = true });
            }

            return BadRequest(new { Success = false, message = "Failed to delete community post" });
        }

        [HttpDelete("DeleteCommunity/{communityId}/{isDeleted}")]
        public async Task<IActionResult> DeleteCommunity(int communityId, bool isDeleted)
        {
            if (await _communityServices.CommunityIsDeletedAsync(communityId, isDeleted))
            {
                return Ok(new { Success = true });
            }

            return BadRequest(new { Success = false, message = "Failed to delete community" });
        }






    }
}