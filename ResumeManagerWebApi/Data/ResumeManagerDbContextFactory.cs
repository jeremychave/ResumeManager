using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ResumeManagerWebApi.Data
{
    public class ResumeManagerDbContextFactory : IDesignTimeDbContextFactory<ResumeManagerDbContext>
    {
        public ResumeManagerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ResumeManagerDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ResumeManagerDb");

            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.AddInterceptors(new AccessTokenInterceptor());

            return new ResumeManagerDbContext(optionsBuilder.Options);
        }
    }
}