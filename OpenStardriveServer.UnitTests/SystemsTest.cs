using NUnit.Framework;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems;

namespace OpenStardriveServer.UnitTests
{
    public abstract class SystemsTest<T> where T : ISystem
    {
        protected T ClassUnderTest;
        
        [SetUp]
        public void BaseSetUp()
        {
            ClassUnderTest = CreateClassUnderTest();
        }

        protected abstract T CreateClassUnderTest();

        protected Command CommandWithPayload(object payload)
        {
            return new Command
            {
                Payload = Json.Serialize(payload)
            };
        }

        protected CommandResult ExpectedCommandResult<U>(Command command, TransformResult<U> expected)
        {
            return expected.ToCommandResult(command, ClassUnderTest.SystemName);
        }

        protected void AssertCommandResult(CommandResult actual, CommandResult expected)
        {
            Assert.That(actual.CommandId, Is.EqualTo(expected.CommandId));
            Assert.That(actual.Type, Is.EqualTo(expected.Type));
            Assert.That(actual.System, Is.EqualTo(expected.System));
            Assert.That(actual.Payload, Is.EqualTo(expected.Payload));
        }

        protected void TestCommand<U>(string commandName, object payload, TransformResult<U> expected)
        {
            if (!ClassUnderTest.CommandProcessors.ContainsKey(commandName))
            {
                Assert.Fail($"There is no configured processor for command: {commandName}");
            }
            
            var command = CommandWithPayload(payload);
            var expectedCommandResult = expected.ToCommandResult(command, ClassUnderTest.SystemName);

            var result = ClassUnderTest.CommandProcessors[commandName](command);
        
            AssertCommandResult(result, expectedCommandResult);
        }
    }
}