using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Clients;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Clients;

 public class ClientsTransformsTests : WithAnAutomocked<ClientsTransforms>
{
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
            Name = "Test Client",
            ClientType = "test-client"
        };

        var result = ClassUnderTest.RegisterClient(priorState, payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.StateChanged));
        Assert.That(result.NewState.Value, Is.Not.SameAs(priorState));
        Assert.That(result.NewState.Value.Clients, Is.Not.SameAs(priorState.Clients));
        Assert.That(result.NewState.Value.Clients.Count, Is.EqualTo(2));
        Assert.That(result.NewState.Value.Clients[0], Is.SameAs(priorState.Clients[0]));
        Assert.That(result.NewState.Value.Clients[1].ClientId, Is.EqualTo(payload.ClientId));
        Assert.That(result.NewState.Value.Clients[1].ClientSecret, Is.EqualTo(payload.ClientSecret));
        Assert.That(result.NewState.Value.Clients[1].Name, Is.EqualTo(payload.Name));
        Assert.That(result.NewState.Value.Clients[1].ClientType, Is.EqualTo(payload.ClientType));
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

        var result = ClassUnderTest.RegisterClient(new ClientsState(), payload);
            
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

        var result = ClassUnderTest.RegisterClient(new ClientsState(), payload);
            
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

        var result = ClassUnderTest.RegisterClient(new ClientsState(), payload);
            
        Assert.That(result.ResultType, Is.EqualTo(TransformResultType.Error));
        Assert.That(result.ErrorMessage, Is.EqualTo("Invalid name"));
    }

    [Test]
    public void When_setting_operator()
    {
        var clientId = NewGuid();
        var payload = new ClientOperatorPayload
        {
            ClientId = clientId,
            Operator = "Allan"
        };
        var state = new ClientsState
        {
            Clients = new()
            {
                new() { ClientId = clientId }
            }
        };

        var result = ClassUnderTest.SetOperator(state, payload);
        
        Assert.That(result.NewState.Value.Clients[0].Operator, Is.EqualTo(payload.Operator));
    }
    
    
    [Test]
    public void When_setting_operator_but_no_matching_clientId()
    {
        var payload = new ClientOperatorPayload
        {
            ClientId = NewGuid(),
            Operator = "Allan"
        };
        var state = new ClientsState();

        var result = ClassUnderTest.SetOperator(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo($"Unable to find client with clientId: {payload.ClientId}"));
    }
    
    [Test]
    public void When_setting_current_screen()
    {
        var clientId = NewGuid();
        var payload = new ClientCurrentScreenPayload
        {
            ClientId = clientId,
            CurrentScreen = "engines"
        };
        var state = new ClientsState
        {
            Clients = new()
            {
                new() { ClientId = clientId }
            }
        };

        var result = ClassUnderTest.SetCurrentScreen(state, payload);
        
        Assert.That(result.NewState.Value.Clients[0].CurrentScreen, Is.EqualTo(payload.CurrentScreen));
    }
    
    
    [Test]
    public void When_setting_current_screen_but_no_matching_clientId()
    {
        var payload = new ClientCurrentScreenPayload
        {
            ClientId = NewGuid(),
            CurrentScreen = "shields"
        };
        var state = new ClientsState();

        var result = ClassUnderTest.SetCurrentScreen(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo($"Unable to find client with clientId: {payload.ClientId}"));
    }
    
    [Test]
    public void When_setting_disabled()
    {
        var clientId = NewGuid();
        var payload = new DisableClientPayload
        {
            ClientId = clientId,
            Disabled = true,
            DisabledMessage = "SYSTEM OFFLINE"
        };
        var state = new ClientsState
        {
            Clients = new()
            {
                new() { ClientId = clientId }
            }
        };

        var result = ClassUnderTest.DisableClient(state, payload);
        
        Assert.That(result.NewState.Value.Clients[0].Disabled, Is.EqualTo(payload.Disabled));
        Assert.That(result.NewState.Value.Clients[0].DisabledMessage, Is.EqualTo(payload.DisabledMessage));
    }
    
    
    [Test]
    public void When_setting_disabled_but_no_matching_clientId()
    {
        var payload = new DisableClientPayload
        {
            ClientId = NewGuid(),
            Disabled = true,
            DisabledMessage = "STATION OFFLINE"
        };
        var state = new ClientsState();

        var result = ClassUnderTest.DisableClient(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo($"Unable to find client with clientId: {payload.ClientId}"));
    }
}