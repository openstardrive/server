using System;

namespace OpenStardriveServer.Domain.Systems
{
    public abstract record SystemBaseState
    {
        public int CurrentPower { get; init; }
        public int RequiredPower { get; init; }
        public bool Disabled { get; init; }
        public bool Damaged { get; init; }

        public Maybe<string> HasInsufficientPower() => (CurrentPower < RequiredPower).MaybeIf("insufficient power");
        public Maybe<string> IsDisabled() => Disabled.MaybeIf("system disabled");
        public Maybe<string> IsDamaged() => Damaged.MaybeIf("system damaged");
        
        public TransformResult<T> IfFunctional<T>(Func<T> stateChange)
        {
            return IsDisabled().OrElse(IsDamaged).OrElse(HasInsufficientPower).Case(
                some: TransformResult<T>.Error,
                none: () => TransformResult<T>.StateChanged(stateChange()));
        }
    }
}