using System;
using NUnit.Framework;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Chronometer;

namespace OpenStardriveServer.UnitTests.Domain.Chronometer
{
    public class IncrementChronometerCommandTests : WithAnAutomocked<IncrementChronometerCommand>
    {
        [Test]
        public void When_incrementing()
        {
            Command savedCommand = null;
            GetMock<ICommandRepository>()
                .Setup(x => x.Save(Any<Command>()))
                .Callback<Command>(x => savedCommand = x);
            
            ClassUnderTest.SetLastTimeForTesting(DateTimeOffset.UtcNow.AddSeconds(-1));
            ClassUnderTest.Increment();

            Assert.That(savedCommand, Is.Not.Null);
            Assert.That(savedCommand.Type, Is.EqualTo(ChronometerCommand.Type));
            Assert.That(savedCommand.TimeStamp, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));

            var payload = Json.Deserialize<ChronometerPayload>(savedCommand.Payload);
            Assert.That(payload.ElapsedMilliseconds, Is.EqualTo(1000).Within(500));
        }
    }
}