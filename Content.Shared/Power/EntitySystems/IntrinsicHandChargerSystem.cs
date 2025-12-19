using System.Diagnostics.CodeAnalysis;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Power.Components;
using Content.Shared.PowerCell;
using Robust.Shared.Timing;

namespace Content.Shared.Power.EntitySystems;

public class IntrinsicHandChargerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedChargesSystem _charges = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
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

    private void ChargeItemBatteries(EntityUid item, IntrinsicHandChargerComponent handCharger)
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

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var currentGameTime = _gameTiming.CurTime;

        var query = EntityQueryEnumerator<IntrinsicHandChargerComponent>();
        while (query.MoveNext(out var uid, out var intrinsicCharger))
        {
            if (intrinsicCharger.NextChargeTime > currentGameTime)
                continue;

            intrinsicCharger.NextChargeTime += intrinsicCharger.ChargeInterval;

            ChargeInHandItems((uid, intrinsicCharger));
        }
    }

    /// <summary>
    /// Attempts to charge in-hand items
    /// </summary>
    private void ChargeInHandItems(Entity<IntrinsicHandChargerComponent> ent)
    {
        if (!TryComp<HandsComponent>(ent, out var hands))
            return;

        foreach (var hand in ent.Comp.HandChargers)
        {
            // TryGetHeldItem internally checks if the hand exists as well
            if (!_hands.TryGetHeldItem((ent, hands), hand, out var item))
                continue;

            ChargeItemBatteries(item.Value, ent.Comp);
            ReplenishItemCharges(item.Value, ent.Comp);
        }
    }

    /// <summary>
    /// Replenishes the charges of items with limitedcharges components
    /// </summary>
    private void ReplenishItemCharges(EntityUid item, IntrinsicHandChargerComponent handChargerComponent)
    {
        if (!TryComp<LimitedChargesComponent>(item, out var charges))
            return;

        _charges.AddCharges((item, charges), handChargerComponent.LimitedChargesAddAmount);
    }
}
