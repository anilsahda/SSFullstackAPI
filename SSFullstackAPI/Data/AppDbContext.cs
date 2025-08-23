using Microsoft.EntityFrameworkCore;
using SSFullstackAPI.Data.Entities;

namespace SSFullstackAPI.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Country> Countries { get; set; }
    }
}
