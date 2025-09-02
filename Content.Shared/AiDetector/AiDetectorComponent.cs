using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.AiDetector;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class AiDetectorComponent : Component
{
    /// <summary>
    /// Whether this detector is active (and will receive target positions)
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Active;

    /// <summary>
    /// Maximum range at which a target can be before it does not report to this detector
    /// </summary>
    [DataField]
    public float MaxRange = 50f;

    /// <summary>
    /// Color of the overlay when a region is within AI view
    /// </summary>
    [DataField]
    public Color AiViewRangeColor = Color.FromHex("#6495ed");

    /// <summary>
    /// Color of the overlay when a region is within the warning buffer around an AI view
    /// </summary>
    [DataField]
    public Color AiViewWarningColor = Color.FromHex("#c46f00");
}
