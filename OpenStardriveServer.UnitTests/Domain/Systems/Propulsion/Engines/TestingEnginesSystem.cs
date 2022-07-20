using OpenStardriveServer.Domain.Systems.Propulsion.Engines;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Propulsion.Engines;

public class TestingEnginesSystem : EnginesSystem
{
    public TestingEnginesSystem(IEnginesTransformations transformations)
        : base("testing-engines", EnginesStateDefaults.Testing , transformations)
    { }
}