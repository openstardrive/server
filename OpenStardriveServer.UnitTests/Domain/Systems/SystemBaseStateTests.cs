using NUnit.Framework;

namespace OpenStardriveServer.UnitTests.Domain.Systems
{
    public class SystemBaseStateTests
    {
        [TestCase(0, true)]
        [TestCase(1, false)]
        [TestCase(2, false)]
        public void When_checking_for_insufficient_power(int power, bool insufficient)
        {
            var state = new SystemBaseStateForTesting
            {
                RequiredPower = 1,
                CurrentPower = power
            };

            var result = state.HasInsufficientPower();
            
            Assert.That(result.HasValue, Is.EqualTo(insufficient));
            result.IfSome(x => Assert.That(x, Is.EqualTo("insufficient power")));
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void When_checking_if_disabled(bool disabled)
        {
            var state = new SystemBaseStateForTesting
            {
                Disabled = disabled
            };

            var result = state.IsDisabled();
            
            Assert.That(result.HasValue, Is.EqualTo(disabled));
            result.IfSome(x => Assert.That(x, Is.EqualTo("system disabled")));
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void When_checking_if_damaged(bool damaged)
        {
            var state = new SystemBaseStateForTesting
            {
                Damaged = damaged
            };

            var result = state.IsDamaged();
            
            Assert.That(result.HasValue, Is.EqualTo(damaged));
            result.IfSome(x => Assert.That(x, Is.EqualTo("system damaged")));
        }
    }
}