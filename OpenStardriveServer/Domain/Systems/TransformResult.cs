namespace OpenStardriveServer.Domain.Systems
{
    public class TransformResult<T>
    {
        public static TransformResult<T> StateChanged(T newState) => new()
        {
            ResultType = TransformResultType.StateChanged,
            NewState = Maybe<T>.Some(newState)
        };
        
        public static TransformResult<T> NoChange() => new()
        {
            ResultType = TransformResultType.NoChange
        };
        
        public static TransformResult<T> Error(string message) => new()
        {
            ResultType = TransformResultType.Error,
            ErrorMessage = message
        };

        private TransformResult() { }

        public TransformResultType ResultType { get; private set; }
        public Maybe<T> NewState { get; private set; }
        public string ErrorMessage { get; private set; } = "";

        public CommandResult ToCommandResult(Command command, string system)
        {
            if (ResultType == TransformResultType.NoChange)
            {
                return CommandResult.NoChange(command, system);
            }
            return NewState.Case(
                some: x => CommandResult.StateChanged(command, system, x),
                none: () => CommandResult.Error(command, system, ErrorMessage));
        }
    }

    public enum TransformResultType
    {
        StateChanged,
        NoChange,
        Error
    }
}