namespace Content.Shared.Silicons.Bots.SecBot;

/// <summary>
/// Component to indicate some form of a security threat
/// Used by security bots to determine who to stuncuff
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
