using Microsoft.EntityFrameworkCore;
using ResumeManagerWebApi.Data.Entities;

namespace ResumeManagerWebApi.Data
{
    public class ResumeManagerDbContext : DbContext
    {
        public ResumeManagerDbContext(DbContextOptions<ResumeManagerDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}
