namespace study_buddys_backend_v2.Models
{
    public class CommunityMemberModel
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; }
        public string Role { get; set; } = "student"; // Default role
    }
}