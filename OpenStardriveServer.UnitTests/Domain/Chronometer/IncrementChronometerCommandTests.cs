using System;
using System.Threading.Tasks;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Chronometer;

namespace OpenStardriveServer.UnitTests.Domain.Chronometer;

public class IncrementChronometerCommandTests : WithAnAutomocked<IncrementChronometerCommand>
{
    [Test]
    public async Task When_incrementing()
    {
        Command savedCommand = null;
        IncrementChronometerPayload serializedData = null;
        GetMock<ICommandRepository>()
            .Setup(x => x.Save(Any<Command>()))
            .Callback<Command>(x => savedCommand = x);
        GetMock<IJson>().Setup(x => x.Serialize(Any<object>()))
            .Callback<object>(x => serializedData = x as IncrementChronometerPayload)
            .Returns("test-json");
            
        ClassUnderTest.SetLastTimeForTesting(DateTimeOffset.UtcNow.AddSeconds(-1));
        await ClassUnderTest.Increment();

        Assert.That(savedCommand, Is.Not.Null);
        Assert.That(savedCommand.Type, Is.EqualTo(ChronometerCommand.Type));
        Assert.That(savedCommand.TimeStamp, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
        Assert.That(savedCommand.Payload, Is.EqualTo("test-json"));

        Assert.That(serializedData.ElapsedMilliseconds, Is.EqualTo(1000).Within(500));
    }
}