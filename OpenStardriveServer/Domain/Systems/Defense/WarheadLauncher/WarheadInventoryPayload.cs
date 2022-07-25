using System;

namespace OpenStardriveServer.Domain.Systems.Defense.WarheadLauncher;

public record WarheadInventoryPayload
{
    public WarheadGroup[] Inventory { get; init; } = Array.Empty<WarheadGroup>();
}