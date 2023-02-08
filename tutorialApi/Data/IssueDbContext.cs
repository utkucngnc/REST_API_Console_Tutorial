using Microsoft.EntityFrameworkCore;
using tutorialApi.Models;

namespace tutorialApi.Data
{
    public class IssueDbContext : DbContext
    {
        public IssueDbContext(DbContextOptions<IssueDbContext> options) : base(options) 
        { 
        }

        public DbSet<Issue> Issues { get; set; }
    }
}
