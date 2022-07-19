using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenStardriveServer.Domain.Chronometer;

namespace OpenStardriveServer.HostedServices;

public class ChronometerService : BackgroundService
{
    private readonly IIncrementChronometerCommand incrementChronometerCommand;
    private readonly ILogger<ChronometerService> logger;

    public ChronometerService(IIncrementChronometerCommand incrementChronometerCommand, ILogger<ChronometerService> logger)
    {
        this.incrementChronometerCommand = incrementChronometerCommand;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Chronometer initialized");
        await Task.Yield();
        await Task.Delay(10000, stoppingToken);
        logger.LogInformation("Starting chronometer...");
        while (!stoppingToken.IsCancellationRequested)
        {
            await incrementChronometerCommand.Increment();
            await Task.Delay(1000, stoppingToken);
        }
    }
}