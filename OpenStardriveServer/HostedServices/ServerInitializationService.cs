using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Database;
using OpenStardriveServer.Domain.Systems;

namespace OpenStardriveServer.HostedServices;

public class ServerInitializationService : BackgroundService
{
    private readonly IRegisterSystemsCommand registerSystemsCommand;
    private readonly ISqliteDatabaseInitializer sqliteDatabaseInitializer;
    private readonly ILogger<ServerInitializationService> logger;
    private readonly ICommandRepository commandRepository;

    public ServerInitializationService(IRegisterSystemsCommand registerSystemsCommand,
        ISqliteDatabaseInitializer sqliteDatabaseInitializer,
        ILogger<ServerInitializationService> logger,
        ICommandRepository commandRepository)
    {
        this.sqliteDatabaseInitializer = sqliteDatabaseInitializer;
        this.logger = logger;
        this.commandRepository = commandRepository;
        this.registerSystemsCommand = registerSystemsCommand;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Registering systems...");
        registerSystemsCommand.Register();
            
        logger.LogInformation("Initializing database...");
        await sqliteDatabaseInitializer.Initialize();

        await commandRepository.Save(new Command { Type = "report-state" });
            
        logger.LogInformation("Server ready");
    }
}