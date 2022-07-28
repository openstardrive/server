using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems.Propulsion.Engines;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines;

public class TestingEnginesSystem : EnginesSystem
{
    public TestingEnginesSystem(IEnginesTransforms transforms, IJson json)
        : base("testing-engines", EnginesStateDefaults.Testing , transforms, json)
    { }
}