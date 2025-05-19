using Microsoft.EntityFrameworkCore;
using study_buddys_backend_v2.Models;

namespace study_buddys_backend_v2.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<UserModels> Users { get; set; }
        public DbSet<CommunityModel> Communitys { get; set; }
        public DbSet<DirectMessageModel> DirectMessages { get; set; }
        public DbSet<CommunityEventsModel> CommunityEvents { get; set; }


    }
}
