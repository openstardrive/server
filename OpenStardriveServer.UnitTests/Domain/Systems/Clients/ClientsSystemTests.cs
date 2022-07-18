using System;
using NUnit.Framework;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems.Clients;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Clients
{
    public class ClientsSystemTests : SystemsTest<ClientsSystem>
    {
        protected override ClientsSystem CreateClassUnderTest() => new();

        [Test]
        public void When_registering_a_client()
        {
            var payload = new RegisterClientPayload
            {
                ClientId = Guid.NewGuid(),
                ClientSecret = "secret",
                Name = "test client"
            };
            var command = new Command
            {
                Payload = Json.Serialize(payload)
            };
            var expectedResult = new ClientsTransformations()
                .RegisterClient(new ClientsState(), payload)
                .ToCommandResult(command, ClassUnderTest.SystemName);
            
            var result = ClassUnderTest.CommandProcessors["register-client"](command);
            
            AssertCommandResult(result, expectedResult);
        }

        [Test]
        public void When_getting_a_registered_client()
        {
            var payload = new RegisterClientPayload
            {
                ClientId = Guid.NewGuid(),
                ClientSecret = "secret",
                Name = "test client"
            };
            var command = new Command
            {
                Payload = Json.Serialize(payload)
            };
            ClassUnderTest.CommandProcessors["register-client"](command);

            var result = ClassUnderTest.FindClientBySecret(payload.ClientSecret);
            
            Assert.That(result.Value.ClientId, Is.EqualTo(payload.ClientId));
        }
    }
}