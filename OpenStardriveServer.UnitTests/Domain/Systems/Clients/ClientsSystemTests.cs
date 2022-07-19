using System;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems.Clients;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Clients;

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
        TestCommand("register-client", payload,
            new ClientsTransformations().RegisterClient(new ClientsState(), payload));
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
        ClassUnderTest.CommandProcessors["register-client"](new Command
        {
            Payload = Json.Serialize(payload)
        });

        var result = ClassUnderTest.FindClientBySecret(payload.ClientSecret);
            
        Assert.That(result.Value.ClientId, Is.EqualTo(payload.ClientId));
        Assert.That(result.Value.Name, Is.EqualTo(payload.Name));
    }
}