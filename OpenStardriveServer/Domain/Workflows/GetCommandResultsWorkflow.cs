using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenStardriveServer.Domain.Workflows
{
    public interface IGetCommandResultsWorkflow
    {
        Task<GetCommandsResult> GetCommandResults(long cursor);
    }

    public class GetCommandResultsWorkflow : IGetCommandResultsWorkflow
    {
        private readonly ICommandResultRepository commandResultRepository;

        public GetCommandResultsWorkflow(ICommandResultRepository commandResultRepository)
        {
            this.commandResultRepository = commandResultRepository;
        }

        public async Task<GetCommandsResult> GetCommandResults(long cursor)
        {
            var results = (await commandResultRepository.LoadPage(cursor)).ToList();
            return new GetCommandsResult
            {
                Results = results,
                NextCursor = results.LastOrDefault()?.RowId ?? cursor
            };
        }
    }

    public record GetCommandsResult
    {
        public List<CommandResult> Results { get; init; }
        public long NextCursor { get; init; }
    }
}