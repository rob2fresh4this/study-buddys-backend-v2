namespace study_buddys_backend_v2.Models
{
    public class UserModels
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Salt { get; set; }
        public string? Hash { get; set; }

        // Communitys Joined and Requests
        public List<int>? OwnedCommunitys { get; set; } = new List<int>();
        public List<int>? JoinedCommunitys { get; set; } = new List<int>();
        public List<int>? CommunityRequests { get; set; } = new List<int>();

    }
}