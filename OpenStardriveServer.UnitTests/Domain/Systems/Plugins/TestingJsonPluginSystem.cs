using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems.Plugins;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Plugins;

public class TestingJsonPluginSystem: JsonPluginSystem
{
    public TestingJsonPluginSystem(IJsonPluginTransforms transforms, IJson json)
        : base(json, transforms, "test", [ "testKey"])
    { }
}
