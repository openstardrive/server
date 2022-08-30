using System;
using System.Collections.Generic;
using System.Linq;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Comms.LongRange;

public interface ILongRangeTransforms : IStandardTransforms<LongRangeState>
{
    TransformResult<LongRangeState> SetCypher(LongRangeState state, SetCypherPayload payload);
    TransformResult<LongRangeState> UpdateCypherSubstitutions(LongRangeState state, UpdateCypherSubstitutionsPayload payload);
    TransformResult<LongRangeState> SendLongRangeMessage(LongRangeState state, LongRangeMessagePayload payload);
}

public class LongRangeTransforms : ILongRangeTransforms
{
    private readonly IStandardTransforms<LongRangeState> standardTransforms;

    public LongRangeTransforms(IStandardTransforms<LongRangeState> standardTransforms)
    {
        this.standardTransforms = standardTransforms;
    }
    
    public TransformResult<LongRangeState> SetDisabled(LongRangeState state, string systemName, DisabledSystemsPayload payload)
    {
        return standardTransforms.SetDisabled(state, systemName, payload);
    }

    public TransformResult<LongRangeState> SetDamaged(LongRangeState state, string systemName, DamagedSystemsPayload payload)
    {
        return standardTransforms.SetDamaged(state, systemName, payload);
    }

    public TransformResult<LongRangeState> SetCurrentPower(LongRangeState state, string systemName, CurrentPowerPayload payload)
    {
        return standardTransforms.SetCurrentPower(state, systemName, payload);
    }

    public TransformResult<LongRangeState> SetRequiredPower(LongRangeState state, string systemName, RequiredPowerPayload payload)
    {
        return standardTransforms.SetRequiredPower(state, systemName, payload);
    }

    public TransformResult<LongRangeState> SetCypher(LongRangeState state, SetCypherPayload payload)
    {
        var newCypher = new Cypher
        {
            CypherId = payload.CypherId,
            Name = payload.Name,
            Description = payload.Description,
            EncodeSubstitutions = payload.EncodeSubstitutions,
            DecodeSubstitutions = payload.DecodeSubstitutions,
            PercentDecoded = CalculatePercentDecoded(payload.EncodeSubstitutions, payload.DecodeSubstitutions)
        };
        return state.Cyphers.FirstOrNone(x => x.CypherId == payload.CypherId).Case(
            some: _ => TransformResult<LongRangeState>.StateChanged(state with
            {
                Cyphers = state.Cyphers.Replace(x => x.CypherId == payload.CypherId, _ => newCypher).ToArray(),
                LastUpdatedCypher = newCypher
            }),
            none: () => TransformResult<LongRangeState>.StateChanged(state with
            {
                Cyphers = state.Cyphers.Append(newCypher).ToArray(),
                LastUpdatedCypher = newCypher
            }));
    }

    public TransformResult<LongRangeState> UpdateCypherSubstitutions(LongRangeState state, UpdateCypherSubstitutionsPayload payload)
    {
        return state.Cyphers.FirstOrNone(x => x.CypherId == payload.CypherId).Case(
            some: match =>
            {
                var updated = match with
                {
                    DecodeSubstitutions = payload.DecodeSubstitutions,
                    PercentDecoded = CalculatePercentDecoded(match.EncodeSubstitutions, payload.DecodeSubstitutions)
                };
                return TransformResult<LongRangeState>.StateChanged(state with
                {
                    Cyphers = state.Cyphers
                        .Replace(x => x.CypherId == payload.CypherId, _ => updated).ToArray(),
                    LastUpdatedCypher = updated
                });
            }, 
            none: () => TransformResult<LongRangeState>.Error($"No cypher found with cypherId: {payload.CypherId}"));
    }

    public double CalculatePercentDecoded(Substitution[] encode, Substitution[] decode)
    {
        if (encode.Length < 1)
        {
            return 1;
        }
        double total = encode.Length;
        return encode.Count(e => decode.Any(d => e.Change == d.To && e.To == d.Change)) / total;
    }

    public TransformResult<LongRangeState> SendLongRangeMessage(LongRangeState state, LongRangeMessagePayload payload)
    {
        if (!string.IsNullOrEmpty(payload.CypherId) && state.Cyphers.None(x => x.CypherId == payload.CypherId))
        {
            return TransformResult<LongRangeState>.Error($"No cypher found with cypherId: {payload.CypherId}");
        }
        
        var now = DateTimeOffset.UtcNow;
        var sentAt = state.LongRangeMessagesSentAt.GetValueOrDefault(payload.MessageId, now);
        return TransformResult<LongRangeState>.StateChanged(state with
        {
            LongRangeMessagesSentAt = new Dictionary<string, DateTimeOffset>(state.LongRangeMessagesSentAt)
            {
                [payload.MessageId] = sentAt
            },
            LastUpdatedLongRangeMessage = new LongRangeMessage
            {
                MessageId = payload.MessageId,
                Sender = payload.Sender,
                Recipient = payload.Recipient,
                Message = payload.Message,
                CypherId = payload.CypherId,
                Outbound = payload.Outbound,
                SentAt = sentAt,
                LastUpdatedAt = now,
            }
        });
    }
}