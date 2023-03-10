using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Net.Core.Authentication.Entities.Contexts
{
    public class ContextFactory : IDesignTimeDbContextFactory<AuthContext>
    {
        public AuthContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json", optional: false);

            var config = builder.Build();

            var settingsSection = config.GetSection("Settings");
            var settings = new Settings();
            settingsSection.Bind(settings);

            var optionsBuilder = new DbContextOptionsBuilder<AuthContext>()
                .UseMySQL(settings.ConnectionStrings.DefaultConnection); // or you can use option #2 either

            return new AuthContext(optionsBuilder.Options);
        }
    }
}
