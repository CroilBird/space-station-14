namespace Content.Shared.Silicons.Bots;

/// <summary>
/// Component to indicate some form of a security threat
/// Used by security bots to determine who to stuncuff
/// </summary>
[RegisterComponent, AutoGenerateComponentState]
public sealed partial class SecurityThreatComponent : Component
{
    /// <summary>
    /// Sources of threat on this entity
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<int, SecurityThreatSourcePrototype> ThreatSources;
}
