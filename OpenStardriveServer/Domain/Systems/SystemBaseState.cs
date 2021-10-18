namespace OpenStardriveServer.Domain.Systems
{
    public abstract class SystemBaseState
    {
        public int CurrentPower { get; set; }
        public int RequiredPower { get; set; }
        public bool Disabled { get; set; }
        public bool Damaged { get; set; }

        public Maybe<string> HasInsufficientPower() => (CurrentPower < RequiredPower).MaybeIf("insufficient power");
        public Maybe<string> IsDisabled() => Disabled.MaybeIf("system disabled");
        public Maybe<string> IsDamaged() => Damaged.MaybeIf("system damaged");
    }
}