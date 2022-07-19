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

        protected void TestCommand<U>(string commandName, object payload, TransformResult<U> expected)
        {
            if (!ClassUnderTest.CommandProcessors.ContainsKey(commandName))
            {
                Assert.Fail($"There is no configured processor for command: {commandName}");
            }
            
            var command = new Command { Payload = Json.Serialize(payload) };
            var expectedCommandResult = expected.ToCommandResult(command, ClassUnderTest.SystemName);

            var result = ClassUnderTest.CommandProcessors[commandName](command);

            Assert.That(result.CommandId, Is.EqualTo(expectedCommandResult.CommandId));
            Assert.That(result.Type, Is.EqualTo(expectedCommandResult.Type));
            Assert.That(result.System, Is.EqualTo(expectedCommandResult.System));
            Assert.That(result.Payload, Is.EqualTo(expectedCommandResult.Payload));
        }
    }
}