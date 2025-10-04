using Content.Shared.Hands;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared.Inventory;

public sealed class HandItemRefillSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DroppedEvent>(OnDropped);
    }

    private void OnDropped(DroppedEvent ev)
    {
        if (!TryComp<InventoryItemRefillComponent>(ev.User, out var handItemRefill))
            return;

        if (!TryComp(ev.User, out TransformComponent? xform))
            return;

        if (!_prototype.Resolve<ItemRefillPrototype>(handItemRefill.ItemRefills, out var itemRefills))
            return;

        foreach (var handItem in itemRefills.HandItems)
        {
            if (!_hands.TryGetHand(ev.User, handItem.Key, out var hand))
                continue;

            if (!_hands.HandIsEmpty(ev.User, handItem.Key))
                continue;

            var ent = Spawn(handItem.Value, xform.Coordinates);
            _hands.TryPickup(ev.User, ent);
        }
    }
}
