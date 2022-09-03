using System;
using OpenStardriveServer.Domain;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Clients;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Clients;

public class ClientsSystemTests : SystemsTest<ClientsSystem>
{
    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", TransformResult<ClientsState>.StateChanged(new ClientsState()));    
    }
    
    [Test]
    public void When_registering_a_client()
    {
        var payload = new RegisterClientPayload
        {
            ClientId = Guid.NewGuid(),
            ClientSecret = "secret",
            Name = "test client"
        };
        var expected = new ClientsTransforms().RegisterClient(new ClientsState(), payload);
        GetMock<IClientsTransforms>().Setup(x => x.RegisterClient(Any<ClientsState>(), payload))
            .Returns(expected);
        
        TestCommandWithPayload("register-client", payload, expected);
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
        GetMock<IJson>().Setup(x => x.Deserialize<RegisterClientPayload>(Any<string>())).Returns(payload);
        GetMock<IClientsTransforms>()
            .Setup(x => x.RegisterClient(Any<ClientsState>(), payload))
            .Returns(new ClientsTransforms().RegisterClient(new ClientsState(), payload));
        
        ClassUnderTest.CommandProcessors["register-client"](new Command
        {
            Payload = "serialized-payload"
        });
        var result = ClassUnderTest.FindClientBySecret(payload.ClientSecret);
            
        Assert.That(result.Value.ClientId, Is.EqualTo(payload.ClientId));
        Assert.That(result.Value.Name, Is.EqualTo(payload.Name));
    }

    [Test]
    public void When_setting_an_operator()
    {
        var payload = new ClientOperatorPayload();
        var expected = TransformResult<ClientsState>.StateChanged(new ClientsState());
        GetMock<IClientsTransforms>().Setup(x => x.SetOperator(Any<ClientsState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-client-operator", payload, expected);
    }
    
    [Test]
    public void When_setting_current_screen()
    {
        var payload = new ClientCurrentScreenPayload();
        var expected = TransformResult<ClientsState>.StateChanged(new ClientsState());
        GetMock<IClientsTransforms>().Setup(x => x.SetCurrentScreen(Any<ClientsState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-client-screen", payload, expected);
    }
    
    [Test]
    public void When_disabling_a_client()
    {
        var payload = new DisableClientPayload();
        var expected = TransformResult<ClientsState>.StateChanged(new ClientsState());
        GetMock<IClientsTransforms>().Setup(x => x.DisableClient(Any<ClientsState>(), payload)).Returns(expected);
        TestCommandWithPayload("set-client-disabled", payload, expected);
    }
}