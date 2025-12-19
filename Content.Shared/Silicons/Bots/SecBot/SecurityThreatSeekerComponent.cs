using Robust.Shared.Prototypes;

namespace Content.Shared.Silicons.Bots.SecBot;

/// <summary>
/// Component for things that respond to entities with a PotentialSecurityThreatComponent
/// </summary>
[RegisterComponent, AutoGenerateComponentState]
public sealed partial class SecurityThreatSeekerComponent : Component
{
    /// <summary>
    /// The threat level threshold for this entity above which something is considered an actual threat
    /// </summary>
    [DataField, AutoNetworkedField]
    public int ThreatThreshold = 5;
}
