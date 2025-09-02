using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class AiDetectorOverlay : Overlay
{
    private static readonly ProtoId<ShaderPrototype> Shader = "AiDetector";

    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private readonly ShaderInstance _aiDetectorShader;

    private TransformComponent? _playerTransform;
    private Vector2 _lastRelativePosition;

    public AiDetectorOverlay()
    {
        IoCManager.InjectDependencies(this);
        _aiDetectorShader = _prototypeManager.Index(Shader).InstanceUnique();

        // after noir shader / greyscale. These make you unable to distinguish between warning-ai-is-near
        // and ai-is-looking-at-you and that's on you buddy.
        ZIndex = 11;
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (_playerTransform is null)
            return base.BeforeDraw(in args);

        var playerPosition = _playerTransform.LocalPosition;
        _aiDetectorShader.SetParameter("player_position", playerPosition);

        _aiDetectorShader.SetParameter("relative_position", _lastRelativePosition);

        _aiDetectorShader.SetParameter("screen_size", args.Viewport.Size);
        _aiDetectorShader.SetParameter("render_scale", args.Viewport.RenderScale);

        return base.BeforeDraw(in args);
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.ScreenHandle;
        handle.UseShader(_aiDetectorShader);
        handle.DrawRect(args.ViewportBounds, Color.White);
        handle.UseShader(null);
    }

    public void SetPlayerTransform(TransformComponent transformComponent)
    {
        _playerTransform = transformComponent;
    }

    public void SetRelativePosition(Vector2 relativePosition)
    {
        _lastRelativePosition = relativePosition;
    }

    public void SetColors(Color aiDetectorAiViewRangeColor, Color aiDetectorAiViewWarningColor)
    {
        _aiDetectorShader.SetParameter("in_range_color", aiDetectorAiViewRangeColor);
        _aiDetectorShader.SetParameter("warning_range_color", aiDetectorAiViewWarningColor);
    }

    public void SetOverlayTexture(Texture overlayTexture)
    {
        _aiDetectorShader.SetParameter("overlay_texture", overlayTexture);
    }
}
