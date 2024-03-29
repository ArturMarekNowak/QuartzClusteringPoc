using Microsoft.AspNetCore.Mvc;

namespace QuartzClusteringPoC.Controllers;

[ApiController]
[Route("[controller]")]
public class AppInstanceIdController : ControllerBase
{
    private readonly ILogger<AppInstanceIdController> _logger;

    public AppInstanceIdController(ILogger<AppInstanceIdController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public Guid Get()
    {
        return Constants.Constants.ApplicationInstanceId;
    }
}