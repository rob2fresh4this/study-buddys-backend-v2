namespace study_buddys_backend_v2.Models
{
    public class CommunityModel
    {
        public int Id { get; set; }
        public int CommunityOwnerID { get; set; }
        public bool IsCommunityOwner { get; set; }
        public bool CommunityIsPublic { get; set; }
        public bool CommunityIsDeleted { get; set; }
        public string? CommunityOwnerName { get; set; }
        public string? CommunityName { get; set; }
        public string? CommunitySubject { get; set; }
        public int CommunityMemberCount { get; set; }
        public List<CommunityMemberModel>? CommunityMembers { get; set; } = new List<CommunityMemberModel>(); // Change here
        public List<int>? CommunityRequests { get; set; } = new List<int>();
        public string? CommunityDifficulty { get; set; }
        public string? CommunityDescription { get; set; }
    }
}