using Quartz;

namespace FileStorage.Jobs;

[DisallowConcurrentExecution]
public class CalculateUsageJob : IJob
{
    private readonly ILogger<CalculateUsageJob> _logger;

    public CalculateUsageJob(ILogger<CalculateUsageJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Hi");

        return Task.CompletedTask;
    }
}
