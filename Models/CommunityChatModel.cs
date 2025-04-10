namespace study_buddys_backend_v2.Models
{
    public class CommunityChatModel
    {
        public int Id { get; set; }
        public int UserIdSender { get; set; }
        public string UserSenderName { get; set; } = string.Empty; // The name of the user who sent the message
        public string Message { get; set; } = string.Empty; // The message content
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Automatically logs creation time
        public string? MediaUrl { get; set; } // Optional media (image, file, etc.)
        public bool IsEdited { get; set; } = false; // True if the message was edited


        // 🧪 Beta Features - Not required for my MVP
        // public int? ReplyToMessageId { get; set; } // For threading (beta)
        // public Dictionary<string, int> Reactions { get; set; } = new(); // Emoji reactions (beta)
    }
}
