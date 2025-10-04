using Robust.Shared.Prototypes;

namespace Content.Shared.Inventory.Prototypes;

[Prototype]
public sealed partial class ItemRefillPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = string.Empty;

    [DataField]
    public Dictionary<string, EntProtoId> HandItems { get; set; } = new();
}
