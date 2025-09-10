
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Magic.Components;

// Used on whitelist for animate spell/wand
[RegisterComponent, NetworkedComponent]
public sealed partial class AnimateableComponent : Component
{
    [DataField]
    public float Area;

    [DataField]
    public float FallbackArea = 15;

    [DataField]
    public float AreaMultiplier = 1;

    [DataField]
    public float GinormousItemArea = 10;

    /// <summary>
    /// The multiplier for damage depending on density of the animated object
    /// Damage will be density * FixtureDensityDamageMultiplier
    /// </summary>
    [DataField]
    public float AreaDamageMultiplier = 1f;

    /// <summary>
    /// Minimum damage an entity does when animated
    /// </summary>
    [DataField]
    public float MinimumDamage;

    /// <summary>
    /// Maximum damage an entity does when animated
    /// </summary>
    [DataField]
    public float MaximumDamage = 10f;

    [DataField]
    public ProtoId<DamageTypePrototype> DamageType = "Blunt";

    /// <summary>
    /// Multiplier for speed depending on item size/area
    /// Speed will be BaseSpeed + Area * AreaMultiplier * AreaSpeedMultiplier
    /// </summary>
    [DataField]
    public float AreaSpeedMultiplier = -0.1f;

    /// <summary>
    /// Base speed for the animated object speed calculation
    /// </summary>
    [DataField]
    public float BaseSpeed = 1.5f;

    /// <summary>
    /// Minimum speed of an animated object
    /// </summary>
    [DataField]
    public float MinimumSpeed = 0.3f;

    /// <summary>
    /// Maximum speed of an animated object
    /// </summary>
    [DataField]
    public float MaximumSpeed = 1.3f;

    [DataField]
    public float FallbackHealth = 30f;

    [DataField]
    public float AreaHealthMultiplier = 1.5f;

    [DataField]
    public float MinimumHealth = 15f;

    [DataField]
    public float MaximumHealth = 50f;
}
