using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using study_buddys_backend_v2.Models.DTOS;
using study_buddys_backend_v2.Services;

namespace study_buddys_backend_v2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userServices;

        public UserController(UserService userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userServices.GetAllUsersAsync();
            if (users == null || !users.Any())
                return NotFound(new { Success = false, Message = "No users found" });

            return Ok(new { Success = true, Users = users });
        }

        [HttpGet("getUserInfo/{username}")]
        public async Task<IActionResult> GetUserInfo(string username)
        {
            var userInfo = await _userServices.GetAllUserInfoAsync(username);

            if (userInfo == null)
                return NotFound(new { Success = false, Message = "User not found" });

            return Ok(new { Success = true, User = userInfo });
        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userServices.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { Success = false, Message = "User not found" });

            return Ok(new { Success = true, User = user });
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDTO user)
        {
            if (await _userServices.RegisterUser(user)) return Ok(new { Success = true });
            return BadRequest(new { Success = false });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserDTO user)
        {
            string token = await _userServices.LoginUser(user);
            if (token != null) return Ok(new { Success = true, Token = token });
            return BadRequest(new { Success = false });
        }

        [HttpPut("EditUserCommunities/{id}")]
        public async Task<IActionResult> EditUserCommunities(int id, [FromBody] EditCommunitiesDTO editCommunities)
        {
            var success = await _userServices.EditUserCommunitiesAsync(id, editCommunities.OwnedCommunityIds, editCommunities.JoinedCommunityIds, editCommunities.CommunityRequestIds);

            if (!success)
            {
                return BadRequest(new { Success = false, Message = "Failed to update user communities" });
            }
            return Ok(new { Success = true, Message = "User communities updated successfully" });
        }

        [HttpPost("AddCommunityToUser/{id}")]
        public async Task<IActionResult> AddCommunityToUser(int id, int communityId)
        {
            var success = await _userServices.AddCommunityToUserAsync(id, communityId);

            if (!success)
            {
                return BadRequest(new { Success = false, Message = "Failed to add community to user" });
            }
            return Ok(new { Success = true, Message = "Community added to user successfully" });
        }


        [HttpDelete("RemoveCommunityFromUser/{id}/{communityId}")]
        public async Task<IActionResult> RemoveCommunityFromUser(int id, int communityId)
        {
            var success = await _userServices.RemoveCommunityFromUserAsync(id, communityId);

            if (!success)
            {
                return BadRequest(new { Success = false, Message = "Failed to remove community from user" });
            }
            return Ok(new { Success = true, Message = "Community removed from user successfully" });
        }

        
    }
}