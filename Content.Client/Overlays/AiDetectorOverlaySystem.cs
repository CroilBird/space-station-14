using Content.Shared.AiDetector;
using Content.Shared.Overlays;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Overlays;

public sealed partial class AiDetectorOverlaySystem : SharedAiDetectorOverlaySystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    private AiDetectorOverlay _aiDetectorOverlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        _aiDetectorOverlay = new();
        SubscribeNetworkEvent<AiPositionUpdateEvent>(OnPositionUpdate);

        SubscribeLocalEvent<AiDetectorComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<AiDetectorComponent, ComponentRemove>(OnRemove);
    }

    private void OnStartup(Entity<AiDetectorComponent> ent, ref ComponentStartup args)
    {
        UpdateOverlay(ent);
    }

    private void OnRemove(Entity<AiDetectorComponent> ent, ref ComponentRemove args)
    {
        RemoveOverlay();
    }

    protected override void OnOverlayToggle(Entity<AiDetectorComponent> ent, ref AiDetectorOverlayToggleEvent args)
    {
        if (!_gameTiming.IsFirstTimePredicted)
            return;

        if (args.Handled)
            return;

        args.Toggle = true;
        args.Handled = true;

        ent.Comp.Active = !ent.Comp.Active;

        UpdateOverlay(ent);
    }

    private void UpdateOverlay(Entity<AiDetectorComponent> ent)
    {
        if (ent.Comp.Active)
        {
            AddOverlay(ent);
        }
        else
        {
            RemoveOverlay();
        }
    }

    private void AddOverlay(Entity<AiDetectorComponent> ent)
    {
        _overlay.AddOverlay(_aiDetectorOverlay);

        _aiDetectorOverlay.SetColors(ent.Comp.AiViewRangeColor, ent.Comp.AiViewWarningColor);
        _aiDetectorOverlay.SetOverlayTexture(
            _sprite.Frame0(new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/ai_overlay.png"))
        ));

        if (!TryComp(ent, out TransformComponent? transformComponent))
            return;

        _aiDetectorOverlay.SetPlayerTransform(transformComponent);
    }

    private void RemoveOverlay()
    {
        _overlay.RemoveOverlay(_aiDetectorOverlay);
    }


    private void OnPositionUpdate(AiPositionUpdateEvent ev)
    {
        Logger.GetSawmill("console").Log(LogLevel.Info, "ai detector target relative position: " + ev.RelativePosition.X + " " + ev.RelativePosition.Y + "(" + ev.RelativePosition.Length() + ")");
        _aiDetectorOverlay.SetRelativePosition(ev.RelativePosition);
    }
}
