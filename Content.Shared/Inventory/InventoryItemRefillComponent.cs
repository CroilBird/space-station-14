using Content.Shared.Inventory.Prototypes;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared.Inventory;

[RegisterComponent]
public sealed partial class InventoryItemRefillComponent : Component
{
    [DataField]
    public ProtoId<ItemRefillPrototype> ItemRefills;
}
