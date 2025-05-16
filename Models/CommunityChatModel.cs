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
        public bool IsDeleted { get; set; } = false; // True if the message was deleted
        public bool IsPinned { get; set; } = false; // True if the message is pinned
        public bool IsEdited { get; set; } = false; // True if the message was edited
        public int? MessageReplyedToMessageId { get; set; }

        public List<ReactionsDTO>? Reactions { get; set; } = new List<ReactionsDTO>(); // List of reactions to the message
    }
}
