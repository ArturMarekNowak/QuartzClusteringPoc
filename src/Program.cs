using Microsoft.EntityFrameworkCore;
using QuartzClusteringPoC.Database;

namespace QuartzClusteringPoC;

public sealed class Program
{
    public static void Main(string[] args)
    { 
        var host = CreateHostBuilder(args).Build();

        // https://stackoverflow.com/questions/36265827/entity-framework-automatic-apply-migrations
        MigrateDatabase(host);
        
        host.Run();
    }

    private static void MigrateDatabase(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("Starting migration...");
            var context = services.GetRequiredService<QuartzClusteringPoCDbContext>();
            context.Database.SetCommandTimeout(300);
            context.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("Migration processed succesfully");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An error occurred creating the DB");
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) => Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}