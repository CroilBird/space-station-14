using System.Threading;
using System.Threading.Tasks;
using Content.Server.Hands.Systems;
using Content.Shared.Hands.Components;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Interactions;


/// <summary>
/// Swaps to a specified hand
/// </summary>
public sealed partial class SwapToHandOperator : HTNOperator
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    /// <summary>
    /// The hand id of the hand to swap to
    /// </summary>
    [DataField]
    public string Hand;

    public override async Task<(bool Valid, Dictionary<string, object>? Effects)> Plan(NPCBlackboard blackboard, CancellationToken cancelToken)
    {
        if (!_entManager.TryGetComponent<HandsComponent>(blackboard.GetValue<EntityUid>(NPCBlackboard.Owner), out var handsComp))
        {
            return (false, null);
        }

        return (true, new Dictionary<string, object>()
        {
            {
                NPCBlackboard.ActiveHand, Hand
            }
        });
    }

    public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);
        var handSystem = _entManager.System<HandsSystem>();

        // regular checks to see if we are handsy
        if (!_entManager.TryGetComponent<HandsComponent>(owner, out var _))
            return HTNOperatorStatus.Failed;

        // cancel out if the hand is already active
        if (handSystem.GetActiveHand(owner) == Hand)
            return HTNOperatorStatus.Finished;

        // actually try setting the active hand
        if (!handSystem.TrySetActiveHand(owner, Hand))
            return HTNOperatorStatus.Failed;

        blackboard.SetValue(NPCBlackboard.ActiveHand, Hand);

        return HTNOperatorStatus.Finished;
    }
}
