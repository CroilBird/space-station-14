using Content.Shared.AiDetector;
using Content.Shared.Overlays;
// using Content.Shared.Silicons.StationAi;
using Robust.Shared.Timing;

namespace Content.Server.Overlays;

public sealed class AiDetectorOverlaySystem : SharedAiDetectorOverlaySystem
{
    // [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    // [Dependency] private readonly SharedStationAiSystem _stationAi = default!;


    protected override void OnOverlayToggle(Entity<AiDetectorComponent> ent, ref AiDetectorOverlayToggleEvent args)
    {

        if (args.Performer != ent.Owner)
            return;

        args.Toggle = true;
        args.Handled = true;

        ent.Comp.Active = !ent.Comp.Active;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<TransformComponent, AiDetectorTargetComponent>();
        while (query.MoveNext(out var uid, out var xform, out var aiDetectorTarget))
        {
            if (_gameTiming.CurTime < aiDetectorTarget.NextUpdate)
                continue;

            aiDetectorTarget.NextUpdate += aiDetectorTarget.UpdateInterval;

            UpdateAiDetectors((uid, xform, aiDetectorTarget));
        }
    }

    private void UpdateAiDetectors(Entity<TransformComponent, AiDetectorTargetComponent> aiEntity)
    {
        var query = EntityQueryEnumerator<TransformComponent, AiDetectorComponent>();
        while (query.MoveNext(out var detectorUid, out var detectorXform, out var aiDetector))
        {
            if (!aiDetector.Active)
                continue;

            // if (!_stationAi.TryGetCore(aiEntity, out var core))
            //     return;
            //
            // // make sure the station AI is on the same map as the detector
            // // we could ignore this, but then the detection would disappear the moment an AI eye went off-grid
            // if (_transform.GetMap(core.Owner) != _transform.GetMap(detectorUid))
            //     continue;

            // we will use the relative world position to do drawing of the overlay later on, and also it's useful now
            // to check the range. calculate it
            // var relativePosition = _transform.GetWorldPosition(aiEntity.Comp1) - _transform.GetWorldPosition(detectorXform);
            var relativePosition = aiEntity.Comp1.LocalPosition - detectorXform.LocalPosition;

            // check range
            if (relativePosition.Length() > aiDetector.MaxRange)
                continue;

            var ev = new AiPositionUpdateEvent(relativePosition);
            RaiseNetworkEvent(ev, detectorUid);
        }
    }
}
