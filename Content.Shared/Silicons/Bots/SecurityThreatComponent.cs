namespace Content.Shared.Silicons.Bots;

/// <summary>
/// Component to indicate some form of a security threat
/// Used by security bots to determine who to stuncuff
/// </summary>
[RegisterComponent, AutoGenerateComponentState]
public sealed partial class SecurityThreatComponent : Component
{
    /// <summary>
    /// The current threat of an entity
    /// </summary>
    [DataField, AutoNetworkedField]
    public int CurrentThreat;
}
