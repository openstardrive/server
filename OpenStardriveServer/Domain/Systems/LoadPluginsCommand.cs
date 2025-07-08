using Microsoft.Extensions.Logging;
using OpenStardriveServer.Domain.Systems.Plugins;
using OpenStardriveServer.Domain.Systems.Standard;  
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OpenStardriveServer.Domain.Systems
{
    public interface ILoadPluginsCommand
    {
        void Load(string path);
    }
    public class LoadPluginsCommand: ILoadPluginsCommand
    {
        private readonly ISystemsRegistry systemsRegistry;
        private readonly ILogger<ILoadPluginsCommand> logger;
        private readonly IJson json;
        public LoadPluginsCommand(ISystemsRegistry systemsRegistry, ILogger<ILoadPluginsCommand> logger)
        {
            this.systemsRegistry = systemsRegistry;
            this.logger = logger;
            json = new Json();
        }
        public void Load(string path)
        {
            if(!Directory.Exists(path))
            {
                logger.LogWarning("Plugin directory not found");
                return;
            }
            var files = Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories);

            var plugins = new List<JsonPluginSystem>();

            foreach (var file in files)
            {
                try
                {
                    var jsonContent = File.ReadAllText(file);
                    var pluginInfo = json.Deserialize<JsonPluginInfo>(jsonContent);
                    logger.LogInformation("Loading plugin {0} from file {1}", pluginInfo.Name, file);

                    var fields=new List<string>();

                    foreach (var pair in pluginInfo.ExtensionData)
                    {
                       fields.Add(pair.Key);
                    }

                    var pluginSystem = new JsonPluginSystem(json, new JsonPluginTransforms(new StandardTransforms<JsonPluginState>()), pluginInfo.Name,fields);
                    plugins.Add(pluginSystem);

                    foreach (var pair in pluginInfo.ExtensionData)
                    {
                        pluginSystem.CommandProcessors[$"update-{pluginInfo.Name}-{pair.Key}"].Invoke(new Command
                        {
                            Type = $"update-{pluginInfo.Name}-{pair.Key}",
                            Payload = json.Serialize(new UpdateJsonStatePayload
                            {
                                Value = pair.Value
                            })
                        });
                    }
                }
                catch (JsonException e)
                {
                    logger.LogError(e, "Failed to parse plugin file {0}", file);
                }
            }

            systemsRegistry.Register(plugins);
        }
    }
}
