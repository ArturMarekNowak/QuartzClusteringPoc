using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using QuartzClusteringPoC.Database;
using QuartzClusteringPoC.Jobs;

namespace QuartzClusteringPoC;

public sealed class Startup
{
    private IConfiguration Configuration { get; }
        
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
        
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "src" });
        });

        var connectionString = Configuration.GetValue<string>("ConnectionStrings:ConnectionString");
        
        services.AddDbContext<QuartzClusteringPoCDbContext>(options => 
            options.UseNpgsql(connectionString));
        
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });
        services.AddQuartz(q =>
        {
            q.SchedulerId = "AUTO";
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });

            var jobKey = new JobKey("awesome job", "awesome group");
            q.AddJob<TestJob>(j => j
                .WithDescription("my awesome job")
                .StoreDurably(true)
                .RequestRecovery(true)
                .WithIdentity(jobKey)
            );
        
            q.AddTrigger(t => t
                .WithIdentity("Cron Trigger")
                .ForJob(jobKey)
                .WithCronSchedule("0/30 * * * * ?")
                .WithDescription("my awesome cron trigger")
            );
        
            q.UsePersistentStore(s =>
            {
                s.PerformSchemaValidation = true; // default
                s.UseProperties = true; // preferred, but not default
                s.RetryInterval = TimeSpan.FromSeconds(15);
                s.UsePostgres(apo =>
                {
                    apo.ConnectionString = connectionString;
                    // this is the default
                    apo.TablePrefix = "QRTZ_";
                });
                s.UseJsonSerializer();
                s.UseClustering(c =>
                {
                    c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                    c.CheckinInterval = TimeSpan.FromSeconds(10);
                });
            });
        });
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
            opt.AwaitApplicationStarted = true;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "src"));

        app.UseHttpsRedirection();
        app.UseRouting();
            
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}