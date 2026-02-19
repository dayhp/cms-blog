using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CMSBlog.Data
{
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            //optionsBuilder.UseSqlServer(connectionString);
            //return new ApplicationDbContext(optionsBuilder.Options);


            // Try to locate the API project folder (adjust name if your API folder is different)
            var patchProject = Path.Combine(Directory.GetCurrentDirectory(), "../CMSBlog.API");
            Console.WriteLine($"Looking for API project in: {patchProject}");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(patchProject)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder1 = new DbContextOptionsBuilder<ApplicationDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            optionsBuilder1.UseSqlServer(connectionString);
            return new ApplicationDbContext(optionsBuilder1.Options);
        }
    }
}
