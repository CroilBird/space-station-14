namespace Content.Shared.Silicons.Bots;

[RegisterComponent, AutoGenerateComponentState]
public sealed partial class SecurityThreatComponent : Component
{
    /// <summary>
    /// The current threat of an entity
    /// </summary>
    [DataField, AutoNetworkedField]
    public int CurrentThreat;
}
