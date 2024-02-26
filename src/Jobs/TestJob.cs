using Quartz;

namespace QuartzClusteringPoC.Jobs;

public class TestJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Job started by {Constants.Constants.ApplicationInstanceId} at {DateTime.Now}");
        await Task.Delay(15 * 1000);
        Console.WriteLine($"Job ended by {Constants.Constants.ApplicationInstanceId} at {DateTime.Now}");
    }
}