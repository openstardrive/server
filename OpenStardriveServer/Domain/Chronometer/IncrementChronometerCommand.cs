using System;
using System.Threading.Tasks;

namespace OpenStardriveServer.Domain.Chronometer
{
    public interface IIncrementChronometerCommand
    {
        Task Increment();
    }

    public class IncrementChronometerCommand : IIncrementChronometerCommand
    {
        private DateTimeOffset lastTime = DateTimeOffset.UtcNow;

        private readonly ICommandRepository commandRepository;

        public IncrementChronometerCommand(ICommandRepository commandRepository)
        {
            this.commandRepository = commandRepository;
        }

        public async Task Increment()
        {
            var now = DateTimeOffset.UtcNow;
            var elapsedMilliseconds = (long) (now - lastTime).TotalMilliseconds;
            await commandRepository.Save(new Command
            {
                Type = ChronometerCommand.Type,
                Payload = Json.Serialize(new { elapsedMilliseconds })
            });
            lastTime = now;
        }

        public void SetLastTimeForTesting(DateTimeOffset value)
        {
            lastTime = value;
        }
    }
}