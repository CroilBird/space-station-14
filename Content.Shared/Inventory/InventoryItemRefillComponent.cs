using Content.Shared.Inventory.Prototypes;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared.Inventory;

/// <summary>
/// Component that specifies items to magically and automatically refill in an entity's inventory
/// </summary>
[RegisterComponent]
public sealed partial class InventoryItemRefillComponent : Component
{
    /// <summary>
    /// List of item refills to perform. See <see cref="ItemRefillPrototype"/>
    /// </summary>
    [DataField]
    public ProtoId<ItemRefillPrototype> ItemRefills;
}
