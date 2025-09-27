using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Power.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Power.EntitySystems;

public abstract class SharedIntrinsicHandChargerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedChargesSystem _charges = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;

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
    /// Attempts to charge all in-hand items
    /// </summary>
    private void ChargeInHandItems(Entity<IntrinsicHandChargerComponent> ent)
    {
        if (!TryComp<HandsComponent>(ent, out var hands))
            return;

        foreach (var hand in _hands.EnumerateHands((ent, hands)))
        {
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

    /// <summary>
    /// Adds charge to batteries or batteries inside of items
    /// TODO: move to shared entirely once batteries are predicted
    /// </summary>
    protected virtual void ChargeItemBatteries(EntityUid item, IntrinsicHandChargerComponent handCharger)
    {

    }
}
