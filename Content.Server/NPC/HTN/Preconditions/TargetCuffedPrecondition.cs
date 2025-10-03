using Content.Server.Cuffs;
using Content.Shared.Cuffs.Components;

namespace Content.Server.NPC.HTN.Preconditions;

public sealed partial class TargetCuffedPrecondition : HTNPrecondition
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    /// <summary>
    /// The key to look for that specifies the target
    /// </summary>
    [DataField]
    public string Key;

    public override bool IsMet(NPCBlackboard blackboard)
    {
        var cuffable = _entManager.System<CuffableSystem>();

        if (!blackboard.TryGetValue<EntityUid>(Key, out var target, _entManager))
            return false;

        if (!_entManager.TryGetComponent<CuffableComponent>(target, out var cuffComp))
            return false;

        return cuffable.IsCuffed((target, cuffComp));
    }

}
