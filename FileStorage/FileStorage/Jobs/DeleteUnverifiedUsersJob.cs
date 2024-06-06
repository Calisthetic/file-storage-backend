using FileStorage.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace FileStorage.Main.Jobs;

[DisallowConcurrentExecution]
public class DeleteUnverifiedUsersJob : IJob
{
    private readonly ILogger<DeleteUnverifiedUsersJob> _logger;
    private readonly ApiDbContext _context;

    public DeleteUnverifiedUsersJob(ILogger<DeleteUnverifiedUsersJob> logger, ApiDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Unverified users have been deleted");
        await _context.Users.Where(x => x.IsVerify == false && 
        x.CreatedAt < DateTime.Now.AddHours(-1)).ExecuteUpdateAsync(s => s.SetProperty(x => x.PrimaryEmailId, (int?)null));
        await _context.Users.Where(x => x.IsVerify == false && 
        x.CreatedAt < DateTime.Now.AddHours(-1)).ExecuteDeleteAsync();
        
        return;
    }
}
