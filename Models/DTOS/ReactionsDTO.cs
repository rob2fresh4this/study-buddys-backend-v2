namespace study_buddys_backend_v2.Models
{
    public class ReactionsDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string? Reaction { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}