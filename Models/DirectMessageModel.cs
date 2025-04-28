
namespace study_buddys_backend_v2.Models
{
    public class DirectMessageModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; } = null;
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        // Optional: read status
        public bool IsRead { get; set; } = false;
    }
}