using Content.Shared.Stunnable;

namespace Content.Server.NPC.HTN.Preconditions;

public sealed partial class TargetStunnedPrecondition : HTNPrecondition
{
    [Dependency] private readonly IEntityManager _entManager = default!;


    /// <summary>
    /// The key to look for that specifies the target
    /// </summary>
    [DataField]
    public string Key;

    /// <summary>
    /// Whether to invert the condition. If set to true, will be true if the target is NOT stunned
    /// </summary>
    [DataField]
    public bool Invert;

    public override bool IsMet(NPCBlackboard blackboard)
    {
        if (!blackboard.TryGetValue<EntityUid>(Key, out var target, _entManager))
            return false;

        if (Invert)
            return !_entManager.HasComponent<StunnedComponent>(target);

        return _entManager.HasComponent<StunnedComponent>(target);
    }
}
