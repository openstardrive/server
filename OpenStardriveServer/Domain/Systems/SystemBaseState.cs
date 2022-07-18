using System;

namespace OpenStardriveServer.Domain.Systems
{
    public record SystemBaseState
    {
        public int CurrentPower { get; init; }
        public int RequiredPower { get; init; }
        public bool Disabled { get; init; }
        public bool Damaged { get; init; }

        public const string InsufficientPowerError = "insufficient power"; 
        public const string DisabledError = "system disabled"; 
        public const string DamagedError = "system damaged"; 

        public Maybe<string> HasInsufficientPower() => (CurrentPower < RequiredPower).MaybeIf(InsufficientPowerError);
        public Maybe<string> IsDisabled() => Disabled.MaybeIf(DisabledError);
        public Maybe<string> IsDamaged() => Damaged.MaybeIf(DamagedError);
        
        public TransformResult<T> IfFunctional<T>(Func<T> stateChange)
        {
            return IsDisabled().OrElse(IsDamaged).OrElse(HasInsufficientPower).Case(
                some: TransformResult<T>.Error,
                none: () => TransformResult<T>.StateChanged(stateChange()));
        }
    }
}