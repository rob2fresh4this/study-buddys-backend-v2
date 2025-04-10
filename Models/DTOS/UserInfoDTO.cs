using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace study_buddys_backend_v2.Models.DTOS
{
    public class UserInfoDTO
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<int> OwnedCommunitys { get; set; } = new List<int>();
        public List<int> JoinedCommunitys { get; set; } = new List<int>();
        public List<int> CommunityRequests { get; set; } = new List<int>();
    }
}