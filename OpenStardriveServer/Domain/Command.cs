using System;

namespace OpenStardriveServer.Domain;

public class Command
{
    public long RowId { get; set; }
    public Guid CommandId { get; set; } = Guid.NewGuid();
    public Guid ClientId { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;
}