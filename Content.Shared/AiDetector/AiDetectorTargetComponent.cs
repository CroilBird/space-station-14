using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.AiDetector;

[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class AiDetectorTargetComponent : Component
{
    /// <summary>
    /// Size (in grid cells) of the warning buffer around an AI view
    /// </summary>
    [DataField]
    public int WarningBuffer = 10;

    /// <summary>
    /// The frequency at which to update the location on this target
    /// </summary>
    [DataField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(0.2f);

    /// <summary>
    /// The next point at which this target's location gets sent
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextUpdate;
}
