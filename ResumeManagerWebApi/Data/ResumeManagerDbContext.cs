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
        public DbSet<UserDocument> UserDocument => Set<UserDocument>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasMany(entity => entity.Documents)
                    .WithOne()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserDocument>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.BlobName).IsUnique();
                entity.HasIndex(u => u.FileName).IsUnique();
            });
        }
    }
}
