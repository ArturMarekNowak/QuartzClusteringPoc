using Quartz;
using QuartzClusteringPoC.Jobs;

var builder = WebApplication.CreateBuilder();

builder.Services.Configure<QuartzOptions>(options =>
{
    options.Scheduling.IgnoreDuplicates = true; // default: false
    options.Scheduling.OverWriteExistingData = true; // default: true
});
builder.Services.AddQuartz(q =>
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
            s.UsePostgres(sqlServer =>
            {
                sqlServer.ConnectionString = "User ID=postgres;Password=postgres;Host=host.docker.internal;Port=5432;Database=Quartz;";
                // this is the default
                sqlServer.TablePrefix = "QRTZ_";
            });
            s.UseJsonSerializer();
            s.UseClustering(c =>
            {
                c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                c.CheckinInterval = TimeSpan.FromSeconds(10);
            });
        });
});
builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
    opt.AwaitApplicationStarted = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();