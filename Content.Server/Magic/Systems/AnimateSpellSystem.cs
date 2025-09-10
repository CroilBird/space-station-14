using Content.Server.Destructible;
using Content.Server.Destructible.Thresholds;
using Content.Server.Destructible.Thresholds.Triggers;
using Content.Shared.Damage;
using Content.Shared.Item;
using Content.Shared.Magic.Components;
using Content.Shared.Magic.Systems;

namespace Content.Server.Magic.Systems;

public sealed class AnimateSpellSystem : SharedAnimateSpellSystem
{
    protected override void SetAnimatedEntityDamageThreshold(Entity<AnimateComponent> entity)
    {
        if (!TryComp<DestructibleComponent>(entity, out var destructible))
            return;

        if (!TryComp<AnimateableComponent>(entity, out var animateable))
            return;

        DamageTrigger? lowestDamageTrigger = null;

        foreach (var threshold in destructible.Thresholds)
        {
            if (threshold.Trigger is not DamageTrigger { } trigger)
                continue;

            if (lowestDamageTrigger is null || trigger.Damage < lowestDamageTrigger.Damage)
                lowestDamageTrigger = trigger;
        }

        if (lowestDamageTrigger is null)
            return;

        var calculatedHealth = animateable.Area * animateable.AreaMultiplier * animateable.AreaHealthMultiplier;

        if (lowestDamageTrigger.Damage < calculatedHealth)
            return;

        lowestDamageTrigger.Damage = (int)calculatedHealth;
    }
}
