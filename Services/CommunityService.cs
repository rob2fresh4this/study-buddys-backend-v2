using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using study_buddys_backend_v2.Context;
using study_buddys_backend_v2.Hubs;
using study_buddys_backend_v2.Models;

namespace study_buddys_backend_v2.Services
{
    public class CommunityService
    {
        private readonly DataContext _dataContext;
        private readonly IHubContext<CommunityHub> _hubContext;

        public CommunityService(DataContext dataContext, IHubContext<CommunityHub> hubContext)
        {
            _dataContext = dataContext;
            _hubContext = hubContext;
        }


        public async Task<List<object>> GetAllCommunitiesAsync()
        {
            var communities = await _dataContext.Communitys
                .Include(c => c.CommunityChats.Where(chat => !chat.IsDeleted))
                    .ThenInclude(chat => chat.Reactions) // Include reactions if you want to show them
                .Include(c => c.CommunityMembers)
                .Where(c => !c.CommunityIsDeleted)
                .ToListAsync();

            var users = await _dataContext.Users.ToListAsync();

            var result = new List<object>();

            foreach (var community in communities)
            {
                var ownerUser = users.FirstOrDefault(u => u.Id == community.CommunityOwnerID);
                string ownerFullName = ownerUser != null ? $"{ownerUser.FirstName} {ownerUser.LastName}" : "no name was found";

                var enrichedMembers = community.CommunityMembers.Select(member =>
                {
                    var user = users.FirstOrDefault(u => u.Id == member.UserId);
                    return new
                    {
                        id = member.Id,
                        userId = member.UserId,
                        role = member.Role,
                        firstName = user?.FirstName ?? "no name",
                        lastName = user?.LastName ?? "no name"
                    };
                }).ToList();

                var enrichedChats = community.CommunityChats.Select(chat =>
                {
                    var sender = users.FirstOrDefault(u => u.Id == chat.UserIdSender);
                    return new
                    {
                        id = chat.Id,
                        userIdSender = chat.UserIdSender,
                        userSenderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "no name",
                        message = chat.Message,
                        timestamp = chat.Timestamp,
                        mediaUrl = chat.MediaUrl,
                        reactions = chat.Reactions?.Select(r => new
                        {
                            id = r.Id,
                            userId = r.UserId,
                            reaction = r.Reaction,
                            createdAt = r.CreatedAt
                        }).ToList(),
                        isPinned = chat.IsPinned,
                        isEdited = chat.IsEdited,
                        isDeleted = chat.IsDeleted,
                        messageReplyedToMessageId = chat.MessageReplyedToMessageId
                    };
                }).ToList();

                result.Add(new
                {
                    id = community.Id,
                    communityOwnerID = community.CommunityOwnerID,
                    communityIsPublic = community.CommunityIsPublic,
                    communityIsDeleted = community.CommunityIsDeleted,
                    communityOwnerName = ownerFullName,
                    communityName = community.CommunityName,
                    communitySubject = community.CommunitySubject,
                    communityMemberCount = community.CommunityMembers.Count,
                    communityChats = enrichedChats,
                    communityMembers = enrichedMembers,
                    communityRequests = community.CommunityRequests,
                    communityDifficulty = community.CommunityDifficulty,
                    communityDescription = community.CommunityDescription
                });
            }

            return result;
        }




        public async Task<bool> AddCommunityAsync(CommunityModel community)
        {
            await _dataContext.Communitys.AddAsync(community);
            await _hubContext.Clients.Group($"Community-{community.Id}").SendAsync("NewCommunityCreated", community);
            return await _dataContext.SaveChangesAsync() != 0;
        }

        public async Task<bool> UpdateCommunityAsync(CommunityModel community)
        {
            var existingCommunity = await _dataContext.Communitys.FindAsync(community.Id);
            if (existingCommunity == null) return false;

            // Preserve CommunityMembers & CommunityRequests
            community.CommunityMembers = existingCommunity.CommunityMembers;
            community.CommunityRequests = existingCommunity.CommunityRequests;

            // Update only the allowed fields
            _dataContext.Entry(existingCommunity).CurrentValues.SetValues(community);
            await _hubContext.Clients.Group($"Community-{community.Id}").SendAsync("CommunityUpdated", community);
            return await _dataContext.SaveChangesAsync() != 0;
        }


        public async Task<bool> AddMemberToCommunityAsync(int communityId, int userId, string role = "student")
        {
            var community = await _dataContext.Communitys
                .Include(c => c.CommunityMembers) // Make sure members are loaded
                .FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null) return false;

            // Ensure the member is not already in the community
            if (!community.CommunityMembers.Any(m => m.UserId == userId))
            {
                community.CommunityMembers.Add(new CommunityMemberModel
                {
                    UserId = userId,
                    Role = role
                });
                await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("NewMemberAdded", userId);
                return await _dataContext.SaveChangesAsync() != 0;
            }
            return false;
        }



        public async Task<bool> RemoveMemberFromCommunityAsync(int communityId, int userId)
        {
            var community = await GetCommunityByIdAsync(communityId);
            if (community == null) return false;

            if (community.CommunityMembers != null)
            {
                var member = community.CommunityMembers.FirstOrDefault(m => m.UserId == userId);
                if (member != null)
                {
                    community.CommunityMembers.Remove(member);
                    int result = await _dataContext.SaveChangesAsync();
                    await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("MemberRemoved", userId);
                    return result != 0;
                }
            }

            return false;
        }



        public async Task<CommunityModel?> GetCommunityByIdAsync(int communityId)
        {
            return await _dataContext.Communitys
                .Include(c => c.CommunityChats.Where(chats => !chats.IsDeleted)) // Ensure chats are included
                .Include(c => c.CommunityMembers) // Ensure members are included
                .FirstOrDefaultAsync(c => c.Id == communityId);
        }

        public async Task<object?> GetCommunityByIdAsyncNEW(int communityId)
        {
            var community = await _dataContext.Communitys
            .Include(c => c.CommunityChats.Where(chat => !chat.IsDeleted))
                .ThenInclude(chat => chat.Reactions)
            .Include(c => c.CommunityMembers)
            .FirstOrDefaultAsync(c => c.Id == communityId && !c.CommunityIsDeleted); // Exclude deleted communities

            if (community == null) return null;

            var users = await _dataContext.Users.ToListAsync();

            var ownerUser = users.FirstOrDefault(u => u.Id == community.CommunityOwnerID);
            string ownerFullName = ownerUser != null ? $"{ownerUser.FirstName} {ownerUser.LastName}" : "";

            var enrichedMembers = community.CommunityMembers.Select(member =>
            {
                var user = users.FirstOrDefault(u => u.Id == member.UserId);
                return new
                {
                    id = member.Id,
                    userId = member.UserId,
                    role = member.Role,
                    firstName = user?.FirstName ?? "no name",
                    lastName = user?.LastName ?? "no name"
                };
            }).ToList();

            var enrichedChats = community.CommunityChats.Select(chat =>
            {
                var sender = users.FirstOrDefault(u => u.Id == chat.UserIdSender);
                return new
                {
                    id = chat.Id,
                    userIdSender = chat.UserIdSender,
                    userSenderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "no name",
                    message = chat.Message,
                    timestamp = chat.Timestamp,
                    mediaUrl = chat.MediaUrl,
                    reactions = chat.Reactions?.Select(r => new
                    {
                        id = r.Id,
                        userId = r.UserId,
                        reaction = r.Reaction,
                        createdAt = r.CreatedAt
                    }).ToList(),
                    isDeleted = chat.IsDeleted,
                    isPinned = chat.IsPinned,
                    isEdited = chat.IsEdited,
                    messageReplyedToMessageId = chat.MessageReplyedToMessageId
                };
            }).ToList();

            return new
            {
                id = community.Id,
                communityOwnerID = community.CommunityOwnerID,
                communityIsPublic = community.CommunityIsPublic,
                communityIsDeleted = community.CommunityIsDeleted,
                communityOwnerName = ownerFullName,
                communityName = community.CommunityName,
                communitySubject = community.CommunitySubject,
                communityMemberCount = community.CommunityMembers.Count,
                communityChats = enrichedChats,
                communityMembers = enrichedMembers,
                communityRequests = community.CommunityRequests,
                communityDifficulty = community.CommunityDifficulty,
                communityDescription = community.CommunityDescription
            };
        }




        public async Task<bool> AddRequestToCommunityAsync(int communityId, int userId)
        {
            var community = await _dataContext.Communitys.FindAsync(communityId);
            if (community == null) return false;

            // Ensure list is initialized
            if (community.CommunityRequests == null)
            {
                community.CommunityRequests = new List<int>();
            }

            if (!community.CommunityRequests.Contains(userId) && !community.CommunityMembers.Any(m => m.UserId == userId))
            {
                community.CommunityRequests.Add(userId);
                _dataContext.Communitys.Update(community); // Explicit update
                await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("NewJoinRequest", userId);
                return await _dataContext.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> RemoveRequestFromCommunityAsync(int communityId, int userId)
        {
            var community = await _dataContext.Communitys.FindAsync(communityId);
            if (community == null) return false;

            if (community.CommunityRequests != null && community.CommunityRequests.Contains(userId))
            {
                community.CommunityRequests.Remove(userId);
                _dataContext.Communitys.Update(community); // Explicit update
                await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("RequestRemoved", userId);
                return await _dataContext.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> ApproveRequestAsync(int communityId, int userId, bool approve)
        {
            var community = await _dataContext.Communitys.FindAsync(communityId);
            if (community == null) return false;

            if (approve)
            {
                // Add user to members
                bool memberAdded = await AddMemberToCommunityAsync(communityId, userId);
                if (!memberAdded) return false;

                // Remove user from requests
                bool requestRemoved = await RemoveRequestFromCommunityAsync(communityId, userId);
                if (!requestRemoved) return false;
            }
            else
            {
                // Remove user from requests
                bool requestRemoved = await RemoveRequestFromCommunityAsync(communityId, userId);
                if (!requestRemoved) return false;
            }

            // Notify the user about the approval status
            await _hubContext.Clients.User(userId.ToString()).SendAsync("RequestApprovalStatus", approve);

            return true;
        }


        // only for testing purposes
        public async Task<bool> ClearCommunityMembersAsync(int communityId)
        {
            var community = await _dataContext.Communitys
                .Include(c => c.CommunityMembers) // Make sure members are included
                .FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null) return false;

            community.CommunityMembers.Clear(); // Remove all members
            community.CommunityMemberCount = 0; // Reset count

            return await _dataContext.SaveChangesAsync() > 0; // Ensure changes are saved
        }

        public async Task<bool> EditCommunityMemberRoleAsync(int communityId, int userId, string newRole)
        {
            var community = await GetCommunityByIdAsync(communityId);

            if (community == null) return false;

            var member = community.CommunityMembers.FirstOrDefault(m => m.UserId == userId);

            if (member != null)
            {
                // Validate role
                if (!IsValidRole(newRole)) return false;

                member.Role = newRole;

                _dataContext.Entry(member).State = EntityState.Modified;
                await _hubContext.Clients.User(userId.ToString()).SendAsync("RoleUpdated", newRole);

                return await _dataContext.SaveChangesAsync() != 0;
            }

            return false;
        }

        // Helper method for role validation
        private bool IsValidRole(string role)
        {
            var validRoles = new HashSet<string> { "student", "owner", "ta", "teacher" };
            return validRoles.Contains(role.ToLower());
        }

        public async Task<List<CommunityModel>> GetCommunitiesByUserIdAsync(int userId)
        {
            // Fetch communities where the user is a member and the community is not deleted
            var communities = await _dataContext.Communitys
                .Include(c => c.CommunityMembers) // Ensure members are included
                .Where(c => c.CommunityMembers.Any(m => m.UserId == userId) && !c.CommunityIsDeleted) // Exclude deleted communities
                .ToListAsync();

            return communities;
        }



        // Ensure the chat is added to the correct community and saved
        public async Task<bool> CreateCommunityChatAsync(int communityId, CommunityChatModel chat)
        {
            var community = await _dataContext.Communitys
                .Include(c => c.CommunityChats) // Ensure chats are included
                .FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null) return false;

            // If the message is a reply, ensure the referenced message exists
            if (chat.MessageReplyedToMessageId.HasValue)
            {
                var originalMessage = community.CommunityChats
                    .FirstOrDefault(c => c.Id == chat.MessageReplyedToMessageId.Value);

                if (originalMessage == null) return false; // If the original message doesn't exist, return false
            }

            community.CommunityChats.Add(chat);
            await _dataContext.SaveChangesAsync(); // Save changes to the database
            await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("ReceiveNewChat", chat);

            return true;
        }



        public async Task<bool> EditCommunityChatAsync(int communityId, int chatId, string newMessage)
        {
            var community = await _dataContext.Communitys
                .Include(c => c.CommunityChats) // Ensure chats are included
                .FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null) return false;

            var chat = community.CommunityChats.FirstOrDefault(c => c.Id == chatId);
            if (chat != null)
            {
                chat.Message = newMessage;
                chat.IsEdited = true; // Mark as edited

                // Pass the updated chat object to SignalR
                await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("ReceiveUpdatedChat", chat);

                await _dataContext.SaveChangesAsync(); // Save changes to the database
                return true;
            }
            return false;
        }


        public async Task<bool> DeleteCommunityPostAsync(int communityId, int chatId, bool isDeleted)
        {
            var community = await _dataContext.Communitys
                .Include(c => c.CommunityChats) // Ensure chats are included
                .FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null) return false;

            var chat = community.CommunityChats.FirstOrDefault(c => c.Id == chatId);
            if (chat != null)
            {
                chat.IsDeleted = isDeleted;
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("ChatDeletedStatus", chatId, isDeleted);
                return true;
            }
            return false;
        }

        public async Task<bool> PinCommunityPostAsync(int communityId, int chatId, bool isPinned)
        {
            var community = await _dataContext.Communitys
                .Include(c => c.CommunityChats) // Ensure chats are included
                .FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null) return false;

            var chat = community.CommunityChats.FirstOrDefault(c => c.Id == chatId);
            if (chat != null)
            {
                chat.IsPinned = isPinned;
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("ChatPinnedStatus", chatId, isPinned);
                return true;
            }
            return false;
        }

        // This method is used to mark a community as deleted without actually removing it from the database
        public async Task<bool> CommunityIsDeletedAsync(int communityId, bool isDeleted)
        {
            var community = await _dataContext.Communitys.FindAsync(communityId);
            if (community == null) return false;

            community.CommunityIsDeleted = isDeleted;
            _dataContext.Communitys.Update(community); // Explicit update
            await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("CommunityDeletedStatus", communityId, isDeleted);
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddReactionAsync(int communityId, int chatId, int userId, string reaction)
        {
            var community = await _dataContext.Communitys
                .Include(c => c.CommunityChats)
                    .ThenInclude(chat => chat.Reactions)
                .FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null) return false;

            var chat = community.CommunityChats.FirstOrDefault(c => c.Id == chatId);
            if (chat == null) return false;

            // Check if the user already added a reaction to this chat
            if (chat.Reactions.Any(r => r.UserId == userId))
            {
                return false; // User already reacted
            }

            var newReaction = new ReactionsDTO
            {
                PostId = chatId,
                UserId = userId,
                Reaction = reaction,
                CreatedAt = DateTime.UtcNow
            };

            chat.Reactions.Add(newReaction);

            await _dataContext.SaveChangesAsync();

            await _hubContext.Clients.Group($"Community-{communityId}").SendAsync("ReactionAdded", chatId, newReaction);

            return true;
        }

        public async Task<bool> EditAndOrRemoveReactionAsync(int chatId, int userId, string newReaction)
        {
            // Find the community that contains the chat
            var community = await _dataContext.Communitys
                .Include(c => c.CommunityChats)
                    .ThenInclude(chat => chat.Reactions)
                .FirstOrDefaultAsync(c => c.CommunityChats.Any(chat => chat.Id == chatId));

            if (community == null) return false;

            var chat = community.CommunityChats.FirstOrDefault(c => c.Id == chatId);
            if (chat == null) return false;

            var reaction = chat.Reactions.FirstOrDefault(r => r.UserId == userId);
            if (reaction == null)
            {
                // Optionally, add a new reaction if not found
                if (!string.IsNullOrEmpty(newReaction))
                {
                    var newReactionObj = new ReactionsDTO
                    {
                        PostId = chatId,
                        UserId = userId,
                        Reaction = newReaction,
                        CreatedAt = DateTime.UtcNow
                    };
                    chat.Reactions.Add(newReactionObj);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(newReaction))
                {
                    // Remove reaction
                    chat.Reactions.Remove(reaction);
                }
                else
                {
                    // Edit reaction
                    reaction.Reaction = newReaction;
                }
            }

            await _dataContext.SaveChangesAsync();
            await _hubContext.Clients.Group($"Community-{community.Id}").SendAsync("ReactionUpdated", chatId, userId, newReaction);

            return true;
        }

        public async Task<List<CommunityModel>> GetAllCommunitiesUnrestrictedAsync()
        {
            return await _dataContext.Communitys.ToListAsync();
        }

        public async Task<object> GetAllOwnersRequsdtsFromEachCommunityAsync(int userId)
        {
            var communities = await _dataContext.Communitys
                .Include(c => c.CommunityMembers)
                .Where(c => c.CommunityMembers.Any(m => m.UserId == userId && m.Role == "owner"))
                .ToListAsync();

            var users = await _dataContext.Users.ToListAsync();

            var enrichedTable = new List<object>();// this table will have communityID, communityName, communityOwnerName, communityRequests
            foreach (var community in communities)
            {
                var ownerUser = users.FirstOrDefault(u => u.Id == community.CommunityOwnerID);
                string ownerFullName = ownerUser != null ? $"{ownerUser.FirstName} {ownerUser.LastName}" : "no name was found";

                var validRequests = community.CommunityRequests != null
                    ? community.CommunityRequests.Where(r => r >= 0).ToList()
                    : new List<int>();

                enrichedTable.Add(new
                {
                    communityId = community.Id,
                    communityName = community.CommunityName,
                    communityOwnerName = ownerFullName,
                    communityRequestCount = $"there are {validRequests.Count} requests",
                    communityRequestCountNumber = validRequests.Count,
                    communityRequests = validRequests
                });
            }
            return enrichedTable;
        }

    }
}