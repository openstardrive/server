using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Clients;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Clients
{
    public class ClientsTransformationsTests
    {
        private ClientsTransformations classUnderTest = new();
        
        [Test]
        public void When_registering_a_client()
        {
            var priorState = new ClientsState
            {
                Clients = new List<Client>
                {
                    new Client { ClientId = Guid.NewGuid() }
                }
            };
            
            var payload = new RegisterClientPayload
            {
                ClientId = Guid.NewGuid(),
                ClientSecret = "it's a secret to everybody",
                Name = "Test Client"
            };

            var result = classUnderTest.RegisterClient(priorState, payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
            Assert.That(result.NewState.Value, Is.Not.SameAs(priorState));
            Assert.That(result.NewState.Value.Clients, Is.Not.SameAs(priorState.Clients));
            Assert.That(result.NewState.Value.Clients.Count, Is.EqualTo(2));
            Assert.That(result.NewState.Value.Clients[0], Is.SameAs(priorState.Clients[0]));
            Assert.That(result.NewState.Value.Clients[1].ClientId, Is.EqualTo(payload.ClientId));
            Assert.That(result.NewState.Value.Clients[1].ClientSecret, Is.EqualTo(payload.ClientSecret));
            Assert.That(result.NewState.Value.Clients[1].Name, Is.EqualTo(payload.Name));
        }

        [Test]
        public void When_registering_a_client_and_the_client_id_is_the_empty_guid()
        {
            var payload = new RegisterClientPayload
            {
                ClientId = Guid.Empty,
                ClientSecret = "it's a secret to everybody",
                Name = "Test Client"
            };

            var result = classUnderTest.RegisterClient(new ClientsState(), payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid clientId"));
        }

        [TestCase(null)]
        [TestCase("")]
        public void When_registering_a_client_and_the_secret_is_null_or_empty(string secret)
        {
            var payload = new RegisterClientPayload
            {
                ClientId = Guid.NewGuid(),
                ClientSecret = secret,
                Name = "Test Client"
            };

            var result = classUnderTest.RegisterClient(new ClientsState(), payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid clientSecret"));
        }
        
        [TestCase(null)]
        [TestCase("")]
        public void When_registering_a_client_and_the_name_is_null_or_empty(string name)
        {
            var payload = new RegisterClientPayload
            {
                ClientId = Guid.NewGuid(),
                ClientSecret = "grumble grumble",
                Name = name
            };

            var result = classUnderTest.RegisterClient(new ClientsState(), payload);
            
            Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid name"));
        }
    }
}