using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using study_buddys_backend_v2.Context;
using study_buddys_backend_v2.Hubs;
using study_buddys_backend_v2.Models;

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

        public async Task<List<CommunityEventsModel>> GetAllEventsAsync()
        {
            return await _context.CommunityEvents
            .Where(e => !e.EventIsCancelled)
            .Include(e => e.EventOrganizers)
            .Include(e => e.EventParticipants)
            .ToListAsync();
        }

        public async Task<List<CommunityEventsModel>> GetEventsByCommunityIdAsync(int communityId)
        {
            return await _context.CommunityEvents
            .Where(e => e.CommunityId == communityId && !e.EventIsCancelled)
            .Include(e => e.EventOrganizers)
            .Include(e => e.EventParticipants)
            .ToListAsync();
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
    }
}