using OpenStardriveServer.Domain.Systems;
using OpenStardriveServer.Domain.Systems.Defense.EnergyBeam;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.UnitTests.Domain.Systems.Defense.EnergyBeam;

public class EnergyBeamSystemTests : SystemsTest<EnergyBeamSystem>
{
    private readonly TransformResult<EnergyBeamState> expected =
        TransformResult<EnergyBeamState>.StateChanged(new EnergyBeamState());

    [Test]
    public void When_constructed_the_system_name_is_set()
    {
        Assert.That(ClassUnderTest.SystemName, Is.EqualTo("energy-beams"));
    }

    [Test]
    public void When_reporting_state()
    {
        TestCommand("report-state", expected);
    }

    [Test]
    public void When_setting_disabled()
    {
        var payload = new DisabledSystemsPayload();
        GetMock<IEnergyBeamTransforms>().Setup(x => x.SetDisabled(Any<EnergyBeamState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-disabled", payload, expected);
    }
    
    [Test]
    public void When_setting_damaged()
    {
        var payload = new DamagedSystemsPayload();
        GetMock<IEnergyBeamTransforms>().Setup(x => x.SetDamaged(Any<EnergyBeamState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-damaged", payload, expected);
    }
    
    [Test]
    public void When_setting_current_power()
    {
        var payload = new CurrentPowerPayload();
        GetMock<IEnergyBeamTransforms>().Setup(x => x.SetCurrentPower(Any<EnergyBeamState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-power", payload, expected);
    }
    
    [Test]
    public void When_setting_required_power()
    {
        var payload = new RequiredPowerPayload();
        GetMock<IEnergyBeamTransforms>().Setup(x => x.SetRequiredPower(Any<EnergyBeamState>(), ClassUnderTest.SystemName, payload)).Returns(expected);
        TestCommandWithPayload("set-required-power", payload, expected);
    }
    
    [Test]
    public void When_setting_bank_charge()
    {
        var payload = new ChargeEnergyBeamPayload();
        GetMock<IEnergyBeamTransforms>().Setup(x => x.SetBankCharge(Any<EnergyBeamState>(), payload)).Returns(expected);
        TestCommandWithPayload("charge-energy-beam", payload, expected);
    }
    
    [Test]
    public void When_firing()
    {
        var payload = new FireEnergyBeamPayload();
        GetMock<IEnergyBeamTransforms>().Setup(x => x.Fire(Any<EnergyBeamState>(), payload)).Returns(expected);
        TestCommandWithPayload("fire-energy-beam", payload, expected);
    }
    
    [Test]
    public void When_configuring_a_bank()
    {
        var payload = new ConfigureEnergyBeamBankPayload();
        GetMock<IEnergyBeamTransforms>().Setup(x => x.ConfigureBank(Any<EnergyBeamState>(), payload)).Returns(expected);
        TestCommandWithPayload("configure-energy-beam", payload, expected);
    }
    
    [Test]
    public void When_configuring_all_banks()
    {
        var payload = new ConfigureAllEnergyBeamBanksPayload();
        GetMock<IEnergyBeamTransforms>().Setup(x => x.ConfigureAllBanks(Any<EnergyBeamState>(), payload)).Returns(expected);
        TestCommandWithPayload("configure-all-energy-beams", payload, expected);
    }
}