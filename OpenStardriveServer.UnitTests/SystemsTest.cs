using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems;

namespace OpenStardriveServer.UnitTests;

public abstract class SystemsTest<T> : WithAnAutomocked<T> where T : ISystem
{
    protected void TestCommand<U>(string commandName, object payload, TransformResult<U> expected)
    {
        if (!ClassUnderTest.CommandProcessors.ContainsKey(commandName))
        {
            Assert.Fail($"There is no configured processor for command: {commandName}");
        }
        
        var command = new Command { Payload = Json.Serialize(payload) };

        var result = ClassUnderTest.CommandProcessors[commandName](command);

        var expectedCommandResult = expected.ToCommandResult(command, ClassUnderTest.SystemName);
        Assert.That(result.CommandId, Is.EqualTo(expectedCommandResult.CommandId));
        Assert.That(result.Type, Is.EqualTo(expectedCommandResult.Type));
        Assert.That(result.System, Is.EqualTo(expectedCommandResult.System));
        Assert.That(result.Payload, Is.EqualTo(expectedCommandResult.Payload));
    }
}