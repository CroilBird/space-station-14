
using System.ComponentModel.DataAnnotations;
using Content.Shared.Damage;
using Robust.Shared.GameStates;

namespace Content.Shared.Magic.Components;

// Added to objects when they are made animate
[RegisterComponent, NetworkedComponent]
public sealed partial class AnimateComponent : Component
{
    /// <summary>
    /// Whether to use the automatically calculated damage for an animated object
    /// Set this to false if you want to use some entity's custom MeleeWeaponComponent damage
    /// </summary>
    [DataField]
    public bool UseCalculatedDamage = true;

    /// <summary>
    /// Fallback damage to use if the damage calculation cannot find some required components or values to work from
    /// </summary>
    [DataField(required: true)]
    public DamageSpecifier FallbackDamage;

    /// <summary>
    /// The multiplier for damage depending on density of the animated object
    /// Damage will be density * FixtureDensityDamageMultiplier
    /// </summary>
    [DataField]
    public float FixtureDensityDamageMultiplier = 0.1f;

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

    /// <summary>
    /// Multiplier for speed depending on density
    /// Speed will be density * FixtureDensitySpeedMultiplier + FixtureDensitiySpeedBase
    /// </summary>
    [DataField]
    public float FixtureDensitySpeedMultiplier = -0.1f;

    /// <summary>
    /// Base speed for the animated object speed calculation
    /// This may seem high, but the speed will be clamped according to MinumSpeed and MaximumSpeed
    /// Speed will be density * FixtureDensitySpeedMultiplier + FixtureDensitiySpeedBase
    /// </summary>
    [DataField]
    public float FixtureDensitiySpeedBase = 6f;

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
    public float FixtureDensityHealthMultiplier = 1.0f;

    [DataField]
    public float MinimumHealth = 10f;

    [DataField]
    public float MaximumHealth = 50;
}
