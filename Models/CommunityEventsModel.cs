using study_buddys_backend_v2.Models.DTOS;

namespace study_buddys_backend_v2.Models
{
    public class CommunityEventsModel
    {
        public int Id { get; set; }
        public int CommunityId { get; set; }
        public string? EventName { get; set; }
        public string? EventDescription { get; set; }
        public DateTime EventDate { get; set; }
        public string? EventUrl { get; set; }
        public string? EventLocation { get; set; }
        public List<EventOrganizerDTO>? EventOrganizers { get; set; }
        public List<EventParticipantDTO>? EventParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public bool EventIsPublic { get; set; }
        public bool EventIsCancelled { get; set; }
    }
}