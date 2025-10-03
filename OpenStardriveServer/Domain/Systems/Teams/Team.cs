using System;

namespace OpenStardriveServer.Domain.Systems.Teams;

public record Team
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Type { get; init; } // damage, security, medical, etc.
    public string SimulatorId { get; init; }
    public string Priority { get; init; } // low, medium, high
    public TeamLocation Location { get; init; }
    public string Orders { get; init; }
    public Officer[] Officers { get; init; } = Array.Empty<Officer>();
}

public record TeamLocation
{
    public string Id { get; init; }
    public string Name { get; init; }
    public Deck Deck { get; init; }
}

public record Deck
{
    public string Id { get; init; }
    public int Number { get; init; }
    public string Name { get; init; }
}

public record Officer
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Position { get; init; }
    public InventoryItem[] Inventory { get; init; } = Array.Empty<InventoryItem>();
}

public record InventoryItem
{
    public string Id { get; init; }
    public string Name { get; init; }
    public int Count { get; init; }
}