namespace study_buddys_backend_v2.Models.DTOS
{
    public class EditCommunitiesDTO
    {
        public List<int>? OwnedCommunityIds { get; set; }
        public List<int>? JoinedCommunityIds { get; set; }
        public List<int>? CommunityRequestIds { get; set; }
    }
}