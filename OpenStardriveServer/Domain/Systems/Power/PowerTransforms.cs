using System;
using System.Collections.Generic;
using System.Linq;
using OpenStardriveServer.Domain.Chronometer;
using OpenStardriveServer.Domain.Systems.Standard;

namespace OpenStardriveServer.Domain.Systems.Power;

public interface IPowerTransforms
{
    TransformResult<PowerState> Configure(PowerState state, ConfigurePowerPayload payload);
    TransformResult<PowerState> UpdatePower(PowerState state, ChronometerPayload payload);
    TransformResult<PowerState> SetBatteryDamage(PowerState state, BatteryDamagePayload payload);
    TransformResult<PowerState> SetBatteryCharge(PowerState state, BatteryChargePayload payload);
}

public class PowerTransforms : IPowerTransforms
{
    private readonly IRandomNumberGenerator randomNumberGenerator;
    private readonly ISystemsRegistry systemsRegistry;
    private readonly ICommandRepository commandRepository;
    private readonly IJson json;

    public PowerTransforms(IRandomNumberGenerator randomNumberGenerator,
        ISystemsRegistry systemsRegistry,
        ICommandRepository commandRepository,
        IJson json)
    {
        this.randomNumberGenerator = randomNumberGenerator;
        this.systemsRegistry = systemsRegistry;
        this.commandRepository = commandRepository;
        this.json = json;
    }
    
    public TransformResult<PowerState> Configure(PowerState state, ConfigurePowerPayload payload)
    {
        var batteries = state.Batteries.Where((_, i) => i < payload.NumberOfBatteries).ToList();
        var newBatteriesToCreate = payload.NumberOfBatteries - batteries.Count;
        if (newBatteriesToCreate > 0)
        {
            batteries.AddRange(Enumerable.Repeat(new Battery { Charge = payload.NewBatteryCharge}, newBatteriesToCreate));    
        }
        
        return TransformResult<PowerState>.StateChanged(new PowerState
        {
            ReactorOutput = payload.TargetOutput,
            Batteries = batteries.ToArray(),
            Config = new PowerConfiguration
            {
                TargetOutput = payload.TargetOutput,
                ReactorDrift = payload.ReactorDrift,
                NumberOfBatteries = payload.NumberOfBatteries,
                MaxBatteryCharge = payload.MaxBatteryCharge,
                UpdateRateInMilliseconds = payload.UpdateRateInMilliseconds
            },
            MillisecondsUntilNextUpdate = payload.UpdateRateInMilliseconds
        });
    }

    public TransformResult<PowerState> UpdatePower(PowerState state, ChronometerPayload payload)
    {
        var remaining = state.MillisecondsUntilNextUpdate - payload.ElapsedMilliseconds;
        if (remaining > 0)
        {
            return TransformResult<PowerState>.StateChanged(state with { MillisecondsUntilNextUpdate = remaining });
        }
        
        var reactorOutput = CalculateReactorOutput(state);
        var systems = systemsRegistry.GetAllPoweredSystems();
        var draw = systems.Select(x => x.CurrentPower).Sum();
        var (batteries, deficit) = ProcessBatteries(state, reactorOutput - draw);
        if (deficit > 0)
        {
            CutPower(systems, deficit);
        }
        
        return TransformResult<PowerState>.StateChanged(state with
        {
            MillisecondsUntilNextUpdate = state.Config.UpdateRateInMilliseconds - Math.Abs(remaining),
            ReactorOutput = reactorOutput,
            Batteries = batteries
        });
    }

    private int CalculateReactorOutput(PowerState state)
    {
        var drift = state.Config.ReactorDrift;
        return state.Config.TargetOutput + (randomNumberGenerator.RandomInt(drift * 2 + 1) - drift);
    }

    private (Battery[] batteries, int deficit) ProcessBatteries(PowerState state, int batteryUse)
    {
        return batteryUse switch
        {
            0 => (state.Batteries, 0),
            > 0 => ChargeBatteries(state, batteryUse),
            _ => DrainBatteries(state, Math.Abs(batteryUse))
        };
    }

    private static (Battery[] batteries, int deficit) DrainBatteries(PowerState state, int amountToDrain)
    {
        if (state.Batteries.None(x => !x.Damaged && x.Charge > 0))
        {
            return (state.Batteries, amountToDrain);
        }
        
        var batteries = new List<Battery>();
        var remaining = amountToDrain;
        foreach (var battery in state.Batteries)
        {
            var charge = battery.Charge;
            if (!battery.Damaged && battery.Charge >= remaining)
            {
                charge = battery.Charge - remaining;
                remaining = 0;
            }

            if (!battery.Damaged && battery.Charge < remaining)
            {
                charge = 0;
                remaining -= battery.Charge;
            }

            batteries.Add(battery with { Charge = charge });
        }

        return (batteries.ToArray(), remaining);
    }

    private (Battery[] batteries, int deficit) ChargeBatteries(PowerState state, int amountToCharge)
    {
        if (state.Batteries.None(x => !x.Damaged && x.Charge < state.Config.MaxBatteryCharge))
        {
            return (state.Batteries, 0);
        }
        
        var batteries = new List<Battery>();
        var remaining = amountToCharge;
        foreach (var battery in state.Batteries)
        {
            var charge = battery.Charge;
            if (!battery.Damaged)
            {
                charge = battery.Charge + remaining;
                remaining = 0;
                if (charge > state.Config.MaxBatteryCharge)
                {
                    remaining = charge - state.Config.MaxBatteryCharge;
                    charge = state.Config.MaxBatteryCharge;
                }    
            }
            batteries.Add(battery with { Charge = charge });
        }

        return (batteries.ToArray(), 0);
    }

    private void CutPower(List<IPoweredSystem> systems, int deficit)
    {
        var ordered = systems.OrderByDescending(x => x.CurrentPower);
        var payload = new CurrentPowerPayload();
        foreach (var system in ordered)
        {
            if (system.CurrentPower >= deficit)
            {
                payload[system.SystemName] = system.CurrentPower - deficit;
                break;
            }

            if (system.CurrentPower > 0)
            {
                payload[system.SystemName] = 0;
                deficit -= system.CurrentPower;
            }
        }

        commandRepository.Save(new Command
        {
            Type = "set-power",
            Payload = json.Serialize(payload)
        });
    }

    public TransformResult<PowerState> SetBatteryDamage(PowerState state, BatteryDamagePayload payload)
    {
        if (payload.BatteryIndex < 0 || payload.BatteryIndex >= state.Batteries.Length)
        {
            return TransformResult<PowerState>.Error($"Invalid batteryIndex: {payload.BatteryIndex}");
        }
        
        return TransformResult<PowerState>.StateChanged(state with
        {
            Batteries = state.Batteries
                .Replace((_, i) => i == payload.BatteryIndex, x => x with { Damaged = payload.IsDamaged})
                .ToArray()
        });
    }

    public TransformResult<PowerState> SetBatteryCharge(PowerState state, BatteryChargePayload payload)
    {
        if (payload.BatteryIndex < 0 || payload.BatteryIndex >= state.Batteries.Length)
        {
            return TransformResult<PowerState>.Error($"Invalid batteryIndex: {payload.BatteryIndex}");
        }

        if (payload.Charge < 0)
        {
            return TransformResult<PowerState>.Error("Batteries may not be charged less than zero");
        }

        if (payload.Charge > state.Config.MaxBatteryCharge)
        {
            return TransformResult<PowerState>.Error("Batteries may not be charged more than the configured maximum");
        }
        
        return TransformResult<PowerState>.StateChanged(state with
        {
            Batteries = state.Batteries
                .Replace((_, i) => i == payload.BatteryIndex, x => x with { Charge = payload.Charge})
                .ToArray()
        });
    }
}