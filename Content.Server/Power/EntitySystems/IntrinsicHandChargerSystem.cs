using System.Diagnostics.CodeAnalysis;
using Content.Server.Power.Components;
using Content.Server.PowerCell;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;

namespace Content.Server.Power.EntitySystems;

public sealed class IntrinsicHandChargerSystem : SharedIntrinsicHandChargerSystem
{
    [Dependency] private readonly BatterySystem _battery = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;

    private bool TryGetBattery(EntityUid item, [NotNullWhen(true)] out Entity<BatteryComponent>? batteryItem)
    {
        batteryItem = null;

        // case where the item itself is a battery or has an internal one
        // e.g. stun batons, disablers and, well, batteries
        if (TryComp<BatteryComponent>(item, out var internalBattery))
        {
            batteryItem = (item, internalBattery);
            return true;
        }

        // try to get a battery from a potential cell slot
        if (!_powerCell.TryGetBatteryFromSlot(item, out var cellSlotItem, out var cellSlotBattery))
            return false;

        batteryItem = (cellSlotItem.Value, cellSlotBattery);

        return true;
    }

    protected override void ChargeItemBatteries(EntityUid item, IntrinsicHandChargerComponent handCharger)
    {
        if (!TryGetBattery(item, out var battery))
            return;

        if (_battery.IsFull(battery.Value, battery))
            return;

        var toCharge = handCharger.BatteryChargeAmount;
        if (battery.Value.Comp.CurrentCharge + toCharge > battery.Value.Comp.MaxCharge)
            toCharge = battery.Value.Comp.MaxCharge - battery.Value.Comp.CurrentCharge;

        _battery.ChangeCharge(item, toCharge, battery);
    }
}
