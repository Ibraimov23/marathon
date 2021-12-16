using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Marathon> Marathons { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<Runner> Runners { get; set; }
        public DbSet<Recording> Recordings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
    }
}
