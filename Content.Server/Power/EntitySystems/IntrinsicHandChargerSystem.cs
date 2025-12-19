using System.Diagnostics.CodeAnalysis;
using Content.Shared.PowerCell;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Server.Power;

namespace Content.Server.Power.EntitySystems;

public sealed class IntrinsicHandChargerSystem : SharedIntrinsicHandChargerSystem
{
    [Dependency] private readonly PredictedBatterySystem _battery = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;

    private bool TryGetBattery(EntityUid item, [NotNullWhen(true)]  out PredictedBatteryComponent? batteryItem)
    {
        batteryItem = null;

        // case where the item itself is a battery or has an internal one
        // e.g. stun batons, disablers and, well, batteries
        if (TryComp<PredictedBatteryComponent>(item, out var internalBattery))
        {
            batteryItem = internalBattery;
            return true;
        }

        // try to get a battery from a potential cell slot
        if (!_powerCell.TryGetBatteryFromSlot(item, out var cellSlotBattery))
            return false;

        batteryItem = cellSlotBattery;

        return true;
    }

    protected override void ChargeItemBatteries(EntityUid item, IntrinsicHandChargerComponent handCharger)
    {
        if (!TryGetBattery(item, out var battery))
            return;

        if (_battery.IsFull(item))
            return;

        var toCharge = handCharger.BatteryChargeAmount;
        if (battery.LastCharge + toCharge > battery.MaxCharge)
            toCharge = battery.MaxCharge - battery.LastCharge;

        _battery.ChangeCharge(item, toCharge);
    }
}
