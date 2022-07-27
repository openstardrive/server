using System;
using System.Threading.Tasks;

namespace OpenStardriveServer.Domain.Chronometer;

public interface IIncrementChronometerCommand
{
    Task Increment();
}

public class IncrementChronometerCommand : IIncrementChronometerCommand
{
    private DateTimeOffset lastTime = DateTimeOffset.UtcNow;

    private readonly ICommandRepository commandRepository;
    private readonly IJson json;

    public IncrementChronometerCommand(ICommandRepository commandRepository, IJson json)
    {
        this.commandRepository = commandRepository;
        this.json = json;
    }

    public async Task Increment()
    {
        var now = DateTimeOffset.UtcNow;
        var elapsedMilliseconds = (long) (now - lastTime).TotalMilliseconds;
        await commandRepository.Save(new Command
        {
            Type = ChronometerCommand.Type,
            Payload = json.Serialize(new IncrementChronometerPayload(elapsedMilliseconds))
        });
        lastTime = now;
    }

    public void SetLastTimeForTesting(DateTimeOffset value)
    {
        lastTime = value;
    }
}

public record IncrementChronometerPayload(long ElapsedMilliseconds);