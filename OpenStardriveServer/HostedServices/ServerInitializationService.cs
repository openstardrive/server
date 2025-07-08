using System.IO;
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
    private readonly ILoadPluginsCommand loadPluginsCommand;
    private readonly ISqliteDatabaseInitializer sqliteDatabaseInitializer;
    private readonly ILogger<ServerInitializationService> logger;
    private readonly ICommandRepository commandRepository;
    private readonly IHostEnvironment env;

    public ServerInitializationService(IRegisterSystemsCommand registerSystemsCommand,
        ILoadPluginsCommand loadPluginsCommand,
        ISqliteDatabaseInitializer sqliteDatabaseInitializer,
        ILogger<ServerInitializationService> logger,
        ICommandRepository commandRepository,
         IHostEnvironment env)
    {
        this.sqliteDatabaseInitializer = sqliteDatabaseInitializer;
        this.logger = logger;
        this.commandRepository = commandRepository;
        this.registerSystemsCommand = registerSystemsCommand;
        this.loadPluginsCommand = loadPluginsCommand;
        this.env = env;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Registering systems...");
        registerSystemsCommand.Register();

        string pluginPath = Path.Combine(env.ContentRootPath,"../Plugins");
        logger.LogInformation("Loading plugins from folder {0}...", [pluginPath]);
        loadPluginsCommand.Load(pluginPath);


        logger.LogInformation("Initializing database...");
        await sqliteDatabaseInitializer.Initialize();

        await commandRepository.Save(new Command { Type = "report-state" });
            
        logger.LogInformation("Server ready");
    }
}