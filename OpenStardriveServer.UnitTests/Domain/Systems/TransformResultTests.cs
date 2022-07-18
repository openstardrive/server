using NUnit.Framework;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems;

namespace OpenStardriveServer.UnitTests.Domain.Systems
{
    public class TransformResultTests
    {
        [Test]
        public void When_creating_a_state_change()
        {
            var newState = new SystemBaseState();
            var result = TransformResult<SystemBaseState>.StateChanged(newState);

            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
            Assert.That(result.NewState.Value, Is.SameAs(newState));
            Assert.That(result.ErrorMessage, Is.Empty);
        }
        
        [Test]
        public void When_creating_no_change()
        {
            var result = TransformResult<SystemBaseState>.NoChange();

            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.NoChange));
            Assert.That(result.NewState.HasValue, Is.False);
            Assert.That(result.ErrorMessage, Is.Empty);
        }
        
        [Test]
        public void When_creating_an_error()
        {
            var message = "Error message";
            var result = TransformResult<SystemBaseState>.Error(message);

            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
            Assert.That(result.NewState.HasValue, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(message));
        }

        [Test]
        public void When_converting_state_change_to_command_result()
        {
            var command = new Command();
            var systemName = "test-system";
            var newState = new SystemBaseState();
            var result = TransformResult<SystemBaseState>
                .StateChanged(newState)
                .ToCommandResult(command, systemName);
            var expected = CommandResult.StateChanged(command, systemName, newState);
            
            Assert.That(result.CommandId, Is.EqualTo(expected.CommandId));
            Assert.That(result.System, Is.EqualTo(expected.System));
            Assert.That(result.Type, Is.EqualTo(expected.Type));
            Assert.That(result.Payload, Is.EqualTo(expected.Payload));
        }

        [Test]
        public void When_converting_no_change_to_command_result()
        {
            var command = new Command();
            var systemName = "test-system";
            var result = TransformResult<SystemBaseState>
                .NoChange()
                .ToCommandResult(command, systemName);
            var expected = CommandResult.NoChange(command, systemName);
            
            Assert.That(result.CommandId, Is.EqualTo(expected.CommandId));
            Assert.That(result.System, Is.EqualTo(expected.System));
            Assert.That(result.Type, Is.EqualTo(expected.Type));
            Assert.That(result.Payload, Is.EqualTo(expected.Payload));
        }
        
        [Test]
        public void When_converting_error_to_command_result()
        {
            var command = new Command();
            var systemName = "test-system";
            var message = "Error message";
            var result = TransformResult<SystemBaseState>
                .Error(message)
                .ToCommandResult(command, systemName);
            var expected = CommandResult.Error(command, systemName, message);
            
            Assert.That(result.CommandId, Is.EqualTo(expected.CommandId));
            Assert.That(result.System, Is.EqualTo(expected.System));
            Assert.That(result.Type, Is.EqualTo(expected.Type));
            Assert.That(result.Payload, Is.EqualTo(expected.Payload));
        }
    }
}