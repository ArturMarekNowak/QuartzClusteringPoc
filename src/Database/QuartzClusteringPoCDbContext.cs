using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace QuartzClusteringPoC.Database;

public class QuartzClusteringPoCDbContext : DbContext
{
    private readonly string _configuration;

    public QuartzClusteringPoCDbContext(IConfiguration configuration) 
    {
        _configuration = configuration.GetValue<string>("ConnectionStrings:ConnectionString")!;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(_configuration);
    }
}