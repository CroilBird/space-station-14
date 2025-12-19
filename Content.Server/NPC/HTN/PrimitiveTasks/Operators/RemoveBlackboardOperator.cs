using Content.Server.Chat.Systems;
using Content.Shared.Chat;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators;

/// <summary>
/// Removes a specified key from the blackboard
/// </summary>
public sealed partial class RemoveBlackboardOperator : HTNOperator
{
    [DataField(required: true)]
    public string Key = string.Empty;

    public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
    {
        if (!blackboard.Remove<object>(Key))
            return HTNOperatorStatus.Failed;

        return base.Update(blackboard, frameTime);
    }
}
