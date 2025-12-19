using Robust.Shared.Prototypes;

namespace Content.Shared.Inventory.Prototypes;

/// <summary>
/// Prototype that specifies an item or set of items to refil in a particular inventory/hand slot
/// </summary>
[Prototype]
public sealed partial class ItemRefillPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = string.Empty;

    /// <summary>
    /// Items to refill in hands. Key is the hand id, value is the prototype ID of the item to fill
    /// </summary>
    [DataField]
    public Dictionary<string, EntProtoId> HandItems { get; set; } = new();
}
