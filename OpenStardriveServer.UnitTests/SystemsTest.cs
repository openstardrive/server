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

        protected void AssertCommandResult(CommandResult actual, CommandResult expected)
        {
            Assert.That(actual.CommandId, Is.EqualTo(expected.CommandId));
            Assert.That(actual.Type, Is.EqualTo(expected.Type));
            Assert.That(actual.System, Is.EqualTo(expected.System));
            Assert.That(actual.Payload, Is.EqualTo(expected.Payload));
        }
    }
}