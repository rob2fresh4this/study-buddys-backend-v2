namespace study_buddys_backend_v2.Models.DTOS
{
    public class EventOrganizerDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}