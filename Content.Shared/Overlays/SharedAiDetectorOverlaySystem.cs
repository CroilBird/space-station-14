using System.Numerics;
using Content.Shared.Actions;
using Content.Shared.AiDetector;
using Robust.Shared.Serialization;

namespace Content.Shared.Overlays;

public abstract class SharedAiDetectorOverlaySystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AiDetectorComponent, AiDetectorOverlayToggleEvent>(OnOverlayToggle);
    }

    protected abstract void OnOverlayToggle(Entity<AiDetectorComponent> ent, ref AiDetectorOverlayToggleEvent args);
}

[Serializable, NetSerializable]
public sealed class AiPositionUpdateEvent(Vector2 relativePosition) : EntityEventArgs
{
    public Vector2 RelativePosition { get; } = relativePosition;
}

public sealed partial class AiDetectorOverlayToggleEvent : InstantActionEvent;
