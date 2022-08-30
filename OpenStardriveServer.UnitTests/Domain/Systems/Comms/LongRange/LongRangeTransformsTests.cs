using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain.Systems.Comms.LongRange;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Comms.LongRange;

public class LongRangeTransformsTests : StandardTransformsTest<LongRangeTransforms, LongRangeState>
{
    [Test]
    public void When_damaged()
    {
        TestStandardDamaged(new LongRangeState());
    }
    [Test]
    public void When_disabled()
    {
        TestStandardDisabled(new LongRangeState());
    }

    [Test]
    public void When_setting_current_power()
    {
        TestStandardCurrentPower(new LongRangeState());
    }

    [Test]
    public void When_setting_required_power()
    {
        TestStandardRequiredPower(new LongRangeState());
    }

    [TestCase(new[] {"A,B,B", "B,C,B", "C,D,E"}, .3333)]
    [TestCase(new[] {"A,B,B", "B,C,B", "C,D,E", "D,E,D"}, .5)]
    [TestCase(new string[0], 1)]
    public void When_calculating_percent_decoded(string[] input, double expectedPercent)
    {
        var encode = new List<Substitution>();
        var decode = new List<Substitution>();
        foreach (var value in input)
        {
            var parts = value.Split(",");
            encode.Add(new Substitution { Change = parts[0], To = parts[1] });
            decode.Add(new Substitution { Change = parts[1], To = parts[2] });
        }

        var result = ClassUnderTest.CalculatePercentDecoded(encode.ToArray(), decode.ToArray());
        
        Assert.That(result, Is.EqualTo(expectedPercent).Within(0.0001));
    }

    [Test]
    public void When_setting_a_new_cypher()
    {
        var state = new LongRangeState();
        var payload = new SetCypherPayload
        {
            CypherId = RandomString(),
            Name = "Cypher Grade 0",
            Description = "Very Simple",
            EncodeSubstitutions = new Substitution[]
            {
                new() { Change = "A", To = "B"},
                new() { Change = "B", To = "C"},
                new() { Change = "C", To = "A"},
            },
            DecodeSubstitutions = new Substitution[]
            {
                new() { Change = "A", To = "C"},
                new() { Change = "B", To = "B"},
                new() { Change = "C", To = "A"},
            }
        };

        var result = ClassUnderTest.SetCypher(state, payload);
        var newState = result.NewState.Value;
        
        Assert.That(newState.LastUpdatedCypher, Is.EqualTo(newState.Cyphers[0]));
        Assert.That(newState.Cyphers.Length, Is.EqualTo(1));
        Assert.That(newState.Cyphers[0].CypherId, Is.EqualTo(payload.CypherId));
        Assert.That(newState.Cyphers[0].Name, Is.EqualTo(payload.Name));
        Assert.That(newState.Cyphers[0].Description, Is.EqualTo(payload.Description));
        Assert.That(newState.Cyphers[0].EncodeSubstitutions, Is.EqualTo(payload.EncodeSubstitutions));
        Assert.That(newState.Cyphers[0].DecodeSubstitutions, Is.EqualTo(payload.DecodeSubstitutions));
        Assert.That(newState.Cyphers[0].PercentDecoded, Is.EqualTo(ClassUnderTest.CalculatePercentDecoded(payload.EncodeSubstitutions, payload.DecodeSubstitutions)));
    }
    
    [Test]
    public void When_setting_an_updated_cypher()
    {
        var cypherId = RandomString();
        var state = new LongRangeState
        {
            Cyphers = new[]
            {
                new Cypher { CypherId = RandomString() },
                new Cypher { CypherId = cypherId },
                new Cypher { CypherId = RandomString() },
                
            }
        };
        var payload = new SetCypherPayload
        {
            CypherId = cypherId,
            Name = "Cypher Grade 0",
            Description = "Very Simple",
            EncodeSubstitutions = new Substitution[]
            {
                new() { Change = "A", To = "B"},
                new() { Change = "B", To = "C"},
                new() { Change = "C", To = "A"},
            },
            DecodeSubstitutions = new Substitution[]
            {
                new() { Change = "A", To = "C"},
                new() { Change = "B", To = "B"},
                new() { Change = "C", To = "A"},
            }
        };

        var result = ClassUnderTest.SetCypher(state, payload);
        var newState = result.NewState.Value;
        
        Assert.That(newState.LastUpdatedCypher, Is.EqualTo(newState.Cyphers[1]));
        Assert.That(newState.Cyphers.Length, Is.EqualTo(3));
        Assert.That(newState.Cyphers[1].CypherId, Is.EqualTo(payload.CypherId));
        Assert.That(newState.Cyphers[1].Name, Is.EqualTo(payload.Name));
        Assert.That(newState.Cyphers[1].Description, Is.EqualTo(payload.Description));
        Assert.That(newState.Cyphers[1].EncodeSubstitutions, Is.EqualTo(payload.EncodeSubstitutions));
        Assert.That(newState.Cyphers[1].DecodeSubstitutions, Is.EqualTo(payload.DecodeSubstitutions));
        Assert.That(newState.Cyphers[1].PercentDecoded, Is.EqualTo(ClassUnderTest.CalculatePercentDecoded(payload.EncodeSubstitutions, payload.DecodeSubstitutions)));
    }

    [Test]
    public void When_updating_cypher_substitutions()
    {
        var cypherId = RandomString();
        var state = new LongRangeState
        {
            Cyphers = new[]
            {
                new Cypher
                {
                    CypherId = cypherId,
                    EncodeSubstitutions = new[]
                    {
                        new Substitution { Change = "A", To = "X" },
                        new Substitution { Change = "B", To = "Q" }
                    }
                }
            }
        };
        var payload = new UpdateCypherSubstitutionsPayload
        {
            CypherId = cypherId,
            DecodeSubstitutions = new[]
            {
                new Substitution { Change = "X", To = "X" },
                new Substitution { Change = "Q", To = "B" },
            }
        };
        
        var result = ClassUnderTest.UpdateCypherSubstitutions(state, payload);
        var newState = result.NewState.Value;
        
        Assert.That(newState.LastUpdatedCypher, Is.EqualTo(newState.Cyphers[0]));
        Assert.That(newState.Cyphers[0].DecodeSubstitutions, Is.EqualTo(payload.DecodeSubstitutions));
        Assert.That(newState.Cyphers[0].PercentDecoded, Is.EqualTo(ClassUnderTest.CalculatePercentDecoded(state.Cyphers[0].EncodeSubstitutions, payload.DecodeSubstitutions)));
    }
    
    [Test]
    public void When_updating_cypher_substitutions_but_no_matching_cypher_is_found()
    {
        var cypherId = RandomString();
        var state = new LongRangeState();
        var payload = new UpdateCypherSubstitutionsPayload
        {
            CypherId = cypherId,
            DecodeSubstitutions = new[]
            {
                new Substitution { Change = "A", To = "C" },
                new Substitution { Change = "B", To = "B" },
            }
        };
        
        var result = ClassUnderTest.UpdateCypherSubstitutions(state, payload);
        
        Assert.That(result.ErrorMessage, Is.EqualTo($"No cypher found with cypherId: {cypherId}"));
    }

    [Test]
    public void When_sending_a_new_long_range_message()
    {
        var cypherId = RandomString();
        var state = new LongRangeState
        {
            Cyphers = new []
            {
                new Cypher { CypherId = cypherId }
            }
        };
        var payload = new LongRangeMessagePayload
        {
            MessageId = RandomString(),
            Sender = "USS Odyssey",
            Recipient = "Lunar Command",
            CypherId = cypherId,
            Message = "Please confirm the presence of cheese on the moon.",
            Outbound = true
        };

        var result = ClassUnderTest.SendLongRangeMessage(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.LongRangeMessagesSentAt[payload.MessageId], Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
        Assert.That(newState.LastUpdatedLongRangeMessage.MessageId, Is.EqualTo(payload.MessageId));
        Assert.That(newState.LastUpdatedLongRangeMessage.Sender, Is.EqualTo(payload.Sender));
        Assert.That(newState.LastUpdatedLongRangeMessage.Recipient, Is.EqualTo(payload.Recipient));
        Assert.That(newState.LastUpdatedLongRangeMessage.CypherId, Is.EqualTo(payload.CypherId));
        Assert.That(newState.LastUpdatedLongRangeMessage.Message, Is.EqualTo(payload.Message));
        Assert.That(newState.LastUpdatedLongRangeMessage.Outbound, Is.EqualTo(payload.Outbound));
        Assert.That(newState.LastUpdatedLongRangeMessage.SentAt, Is.EqualTo(newState.LongRangeMessagesSentAt[payload.MessageId]));
        Assert.That(newState.LastUpdatedLongRangeMessage.LastUpdatedAt, Is.EqualTo(newState.LongRangeMessagesSentAt[payload.MessageId]));
    }

    [Test]
    public void When_updating_a_long_range_message()
    {
        var messageId = RandomString();
        var sentAt = DateTimeOffset.UtcNow.AddSeconds(-30);
        var state = new LongRangeState
        {
            LongRangeMessagesSentAt = new Dictionary<string, DateTimeOffset>
            {
                [messageId] = sentAt
            }
        };
        var payload = new LongRangeMessagePayload
        {
            MessageId = messageId,
            Sender = "Lunar Command",
            Recipient = "USS Odyssey",
            Message = "Negative. You're thinking of Wallace & Gromit",
            Outbound = false
        };

        var result = ClassUnderTest.SendLongRangeMessage(state, payload);

        var newState = result.NewState.Value;
        Assert.That(newState.LongRangeMessagesSentAt[payload.MessageId], Is.EqualTo(sentAt));
        Assert.That(newState.LastUpdatedLongRangeMessage.MessageId, Is.EqualTo(payload.MessageId));
        Assert.That(newState.LastUpdatedLongRangeMessage.Sender, Is.EqualTo(payload.Sender));
        Assert.That(newState.LastUpdatedLongRangeMessage.Recipient, Is.EqualTo(payload.Recipient));
        Assert.That(newState.LastUpdatedLongRangeMessage.CypherId, Is.EqualTo(payload.CypherId));
        Assert.That(newState.LastUpdatedLongRangeMessage.Message, Is.EqualTo(payload.Message));
        Assert.That(newState.LastUpdatedLongRangeMessage.Outbound, Is.EqualTo(payload.Outbound));
        Assert.That(newState.LastUpdatedLongRangeMessage.SentAt, Is.EqualTo(sentAt));
        Assert.That(newState.LastUpdatedLongRangeMessage.LastUpdatedAt, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }
    
    [Test]
    public void When_sending_a_long_range_message_but_the_cypher_is_not_found()
    {
        var state = new LongRangeState();
        var payload = new LongRangeMessagePayload
        {
            MessageId = RandomString(),
            CypherId = RandomString()
        };

        var result = ClassUnderTest.SendLongRangeMessage(state, payload);

        Assert.That(result.ErrorMessage, Is.EqualTo($"No cypher found with cypherId: {payload.CypherId}"));
    }
}