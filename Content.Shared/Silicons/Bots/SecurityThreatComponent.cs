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

    [DataField, AutoNetworkedField]
    public int MinorContrabandThreat = 1;

    [DataField, AutoNetworkedField]
    public int MajorContrabandThreat = 3;

    [DataField, AutoNetworkedField]
    public int HighlyIllegalContrabandThreat = 5;

    [DataField, AutoNetworkedField]
    public int AgentIdThreat = -10;

    [DataField, AutoNetworkedField]
    public int SecBotEmaggedThreat = 10;
}
