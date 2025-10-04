using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Power.Components;

/// <summary>
/// Component that causes an entity to recharge anything in its hand slots over time
/// </summary>
[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class IntrinsicHandChargerComponent : Component
{
    /// <summary>
    /// List of hands to try charging items in
    /// </summary>
    [DataField]
    public List<string> HandChargers = [];

    /// <summary>
    /// The amount by which to charge batteries, or batteries contained within items
    /// </summary>
    [DataField]
    public float BatteryChargeAmount = 100f;

    /// <summary>
    /// The number of charges to add to a limitedcharges item, e.g. flashes
    /// </summary>
    [DataField]
    public int LimitedChargesAddAmount = 1;

    /// <summary>
    /// The charge interval at which to charge items
    /// </summary>
    [DataField]
    public TimeSpan ChargeInterval = TimeSpan.FromSeconds(3);

    /// <summary>
    /// The next gametime at which to charge an item
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextChargeTime;
}
