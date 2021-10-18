using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems;

namespace OpenStardriveServer.UnitTests.Domain.Systems
{
    public class SystemsRegistryTests : WithAnAutomocked<SystemsRegistry>
    {
        [Test]
        public void When_getting_a_system_by_name_and_that_system_is_not_found()
        {
            var result = ClassUnderTest.GetSystemByName("anything");
            
            Assert.That(result.HasValue, Is.False);
        }

        [Test]
        public void When_getting_a_system_by_name_and_there_are_some_systems()
        {
            var systems = new ISystem[]
            {
                new TestSystem("system-a"), 
                new TestSystem("system-b"), 
                new TestSystem("system-c")
            };
            ClassUnderTest.Register(systems);
            
            var result = ClassUnderTest.GetSystemByName(systems[1].SystemName);
            
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.SameAs(systems[1]));
        }

        [Test]
        public void When_no_systems_have_been_registered_there_are_no_processors()
        {
            var result = ClassUnderTest.GetAllProcessors();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void When_systems_are_registered_you_can_get_all_processors()
        {
            var systems = new ISystem[]
            {
                new TestSystem("system-a"), 
                new TestSystem("system-b")
            };
            ClassUnderTest.Register(systems);

            var result = ClassUnderTest.GetAllProcessors();
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result["test-command"].Count, Is.EqualTo(2));
            Assert.That(result["test-system-a-command"].Count, Is.EqualTo(1));
            Assert.That(result["test-system-b-command"].Count, Is.EqualTo(1));
            
            var command = new Command();
            var invoked = result["test-system-a-command"][0](command);
            Assert.That(invoked.Type, Is.EqualTo(CommandResult.ErrorType));
            Assert.That(invoked.Payload, Is.EqualTo("\"Test system-a error\""));
        }

        [Test]
        public void Processors_are_cached_until_new_systems_are_registered()
        {
            var systems = new []
            {
                new TestSystem("system-a"), 
                new TestSystem("system-b")
            };
            ClassUnderTest.Register(systems);

            var result = ClassUnderTest.GetAllProcessors();
            Assert.That(result.Count, Is.EqualTo(3));
         
            systems[0].TestProcessors["new"] = c => CommandResult.Error(c, "foo", "bar");
            
            result = ClassUnderTest.GetAllProcessors();
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.ContainsKey("new"), Is.False);
            
            ClassUnderTest.Register(new [] { new TestSystem("system-c") });
            result = ClassUnderTest.GetAllProcessors();
            Assert.That(result.Count, Is.EqualTo(5));
            
            Assert.That(result.ContainsKey("new"), Is.True);
        }

        private class TestSystem : ISystem
        {
            public string SystemName { get; }

            public Dictionary<string, Func<Command, CommandResult>> TestProcessors;
            
            public Dictionary<string, Func<Command, CommandResult>> CommandProcessors => TestProcessors ??= new Dictionary<string, Func<Command, CommandResult>>
            {
                ["test-command"] = c => CommandResult.Error(c, SystemName, "Test error"),
                [$"test-{SystemName}-command"] = c => CommandResult.Error(c, SystemName, $"Test {SystemName} error")
            };

            public TestSystem(string name)
            {
                SystemName = name;
            }
        }
    }
}