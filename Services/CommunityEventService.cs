using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using study_buddys_backend_v2.Context;
using study_buddys_backend_v2.Hubs;
using study_buddys_backend_v2.Models;
using study_buddys_backend_v2.Models.DTOS;

namespace study_buddys_backend_v2.Services
{
    public class CommunityEventService
    {
        private readonly DataContext _context;
        private readonly IHubContext<CommunityHub> _hubContext;

        public CommunityEventService(DataContext context, IHubContext<CommunityHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<List<object>> GetAllEventsAsync()
        {
            var events = await _context.CommunityEvents
                .Where(e => !e.EventIsCancelled)
                .Include(e => e.EventOrganizers)
                .Include(e => e.EventParticipants)
                .ToListAsync();

            var users = await _context.Users.ToListAsync();

            var enriched = events.Select(ev => new
            {
                id = ev.Id,
                communityId = ev.CommunityId,
                eventName = ev.EventName,
                eventDescription = ev.EventDescription,
                eventDate = ev.EventDate,
                eventUrl = ev.EventUrl,
                eventLocation = ev.EventLocation,
                maxParticipants = ev.MaxParticipants,
                currentParticipants = ev.CurrentParticipants,
                eventIsPublic = ev.EventIsPublic,
                eventIsCancelled = ev.EventIsCancelled,
                eventOrganizers = ev.EventOrganizers?.Select(org =>
                {
                    var user = users.FirstOrDefault(u => u.Id == org.UserId);
                    return new
                    {
                        userId = org.UserId,
                        firstName = user?.FirstName ?? "no name",
                        lastName = user?.LastName ?? "no name"
                    };
                }).ToList(),
                eventParticipants = ev.EventParticipants?.Select(part =>
                {
                    var user = users.FirstOrDefault(u => u.Id == part.UserId);
                    return new
                    {
                        userId = part.UserId,
                        firstName = user?.FirstName ?? "no name",
                        lastName = user?.LastName ?? "no name"
                    };
                }).ToList()
            }).ToList();

            return enriched.Cast<object>().ToList();
        }

        public async Task<List<object>> GetEventsByCommunityIdAsync(int communityId)
        {
            var events = await _context.CommunityEvents
                .Where(e => e.CommunityId == communityId && !e.EventIsCancelled)
                .Include(e => e.EventOrganizers)
                .Include(e => e.EventParticipants)
                .ToListAsync();

            var users = await _context.Users.ToListAsync();

            var enriched = events.Select(ev => new
            {
                id = ev.Id,
                communityId = ev.CommunityId,
                eventName = ev.EventName,
                eventDescription = ev.EventDescription,
                eventDate = ev.EventDate,
                eventUrl = ev.EventUrl,
                eventLocation = ev.EventLocation,
                maxParticipants = ev.MaxParticipants,
                currentParticipants = ev.CurrentParticipants,
                eventIsPublic = ev.EventIsPublic,
                eventIsCancelled = ev.EventIsCancelled,
                eventOrganizers = ev.EventOrganizers?.Select(org =>
                {
                    var user = users.FirstOrDefault(u => u.Id == org.UserId);
                    return new
                    {
                        userId = org.UserId,
                        firstName = user?.FirstName ?? "no name",
                        lastName = user?.LastName ?? "no name"
                    };
                }).ToList(),
                eventParticipants = ev.EventParticipants?.Select(part =>
                {
                    var user = users.FirstOrDefault(u => u.Id == part.UserId);
                    return new
                    {
                        userId = part.UserId,
                        firstName = user?.FirstName ?? "no name",
                        lastName = user?.LastName ?? "no name"
                    };
                }).ToList()
            }).ToList();

            return enriched.Cast<object>().ToList();
        }

        public async Task<CommunityEventsModel?> CreateEventAsync(CommunityEventsModel eventModel)
        {
            _context.CommunityEvents.Add(eventModel);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group(eventModel.CommunityId.ToString()).SendAsync("EventCreated", eventModel);
            return eventModel;
        }

        public async Task<CommunityEventsModel?> UpdateEventAsync(CommunityEventsModel eventModel)
        {
            var existingEvent = await _context.CommunityEvents.FindAsync(eventModel.Id);
            if (existingEvent == null)
            {
                return null;
            }

            existingEvent.EventName = eventModel.EventName;
            existingEvent.EventDescription = eventModel.EventDescription;
            existingEvent.EventDate = eventModel.EventDate;
            existingEvent.EventUrl = eventModel.EventUrl;
            existingEvent.EventLocation = eventModel.EventLocation;
            existingEvent.MaxParticipants = eventModel.MaxParticipants;
            existingEvent.CurrentParticipants = eventModel.CurrentParticipants;
            existingEvent.EventIsPublic = eventModel.EventIsPublic;
            existingEvent.EventIsCancelled = eventModel.EventIsCancelled;

            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group(eventModel.CommunityId.ToString()).SendAsync("EventUpdated", existingEvent);
            return existingEvent;
        }

        public async Task<bool> DeleteEventAsync(int communityId, int id, bool isDeleted)
        {
            if (isDeleted)
            {
                var eventToDelete = await _context.CommunityEvents
                    .FirstOrDefaultAsync(e => e.CommunityId == communityId && e.Id == id);
                if (eventToDelete == null)
                {
                    return false;
                }

                eventToDelete.EventIsCancelled = true;
                await _context.SaveChangesAsync();
                await _hubContext.Clients.Group(communityId.ToString()).SendAsync("EventDeleted", eventToDelete);
                return true;
            }

            return false;
        }

        public async Task<bool> AddParticipantsAsync(int eventId, int userId)
        {
            var eventToUpdate = await _context.CommunityEvents
                .Include(e => e.EventParticipants)
                .FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventToUpdate == null) return false;

            var userToAdd = await _context.Users.FindAsync(userId);
            if (userToAdd == null) return false;

            // Prevent duplicate participants
            if (eventToUpdate.EventParticipants != null && eventToUpdate.EventParticipants.Any(p => p.UserId == userId))
                return false;

            // Add participant
            var newParticipant = new EventParticipantDTO
            {
                UserId = userId,
            };

            if (eventToUpdate.EventParticipants == null)
                eventToUpdate.EventParticipants = new List<EventParticipantDTO>();

            eventToUpdate.EventParticipants.Add(newParticipant);
            eventToUpdate.CurrentParticipants++;

            await _context.SaveChangesAsync();

            // Optionally notify via SignalR
            await _hubContext.Clients.Group(eventToUpdate.CommunityId.ToString())
                .SendAsync("EventParticipantAdded", eventId, userId);

            return true;
        }
    }
}