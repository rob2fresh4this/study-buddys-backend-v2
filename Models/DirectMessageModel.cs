
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
        public bool IsRead { get; set; } = false;

        // new properties 
        public bool RecieverRead { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public bool IsEdited { get; set; } = false;
        public DateTime? EditedAt { get; set; }
    }
}