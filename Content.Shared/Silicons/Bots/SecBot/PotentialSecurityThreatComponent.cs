using Robust.Shared.Prototypes;

namespace Content.Shared.Silicons.Bots.SecBot;

/// <summary>
/// Component to indicate some form of a security threat
/// Used by security bots to determine who to stuncuff
/// </summary>
[RegisterComponent, AutoGenerateComponentState]
public sealed partial class PotentialSecurityThreatComponent : Component
{
    /// <summary>
    /// Current threat level of this entity
    /// </summary>
    [DataField, AutoNetworkedField]
    public int ThreatLevel = 0;

    [DataField, AutoNetworkedField]
    public Dictionary<EntProtoId, int> ContrabandThreatModifiers = new()
    {
        { "Syndicate", 10 },
        { "Magical", 10 },
        { "HighlyIllegal", 10 },
        { "Major", 5 },
        { "Restricted", 5 },
        { "Minor", 2 }
    };
}
