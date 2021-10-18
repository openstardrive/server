using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenStardriveServer.Domain;

namespace OpenStardriveServer.HostedServices
{
    public class CommandProcessingService : BackgroundService
    {
        private readonly ICommandProcessor commandProcessor;
        private readonly ILogger<CommandProcessingService> logger;

        public CommandProcessingService(ICommandProcessor commandProcessor, ILogger<CommandProcessingService> logger)
        {
            this.commandProcessor = commandProcessor;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting command processor...");
            await Task.Yield();
            while (!stoppingToken.IsCancellationRequested)
            {
                await commandProcessor.ProcessBatch();
                await Task.Delay(200, stoppingToken);
            }
            logger.LogInformation("Command processor shutdown");
        }
    }
}