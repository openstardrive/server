using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines;

public class TestingEnginesSystem : EnginesSystem
{
    public TestingEnginesSystem(IEnginesTransformations transformations, IJson json)
        : base("testing-engines", EnginesStateDefaults.Testing , transformations, json)
    { }
}