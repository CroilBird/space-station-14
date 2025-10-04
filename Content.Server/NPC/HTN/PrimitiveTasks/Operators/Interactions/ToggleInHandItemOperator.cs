using Content.Server.Hands.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Interactions;

/// <summary>
/// Toggles a toggleable item in the active hand, if any, to a specified state
/// </summary>
public sealed partial class ToggleInHandItemOperator : HTNOperator
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    /// <summary>
    /// The state to toggle the active hand item to. True = on, False = off
    /// </summary>
    [DataField]
    public bool Toggle;

    public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        var handSystem = _entManager.System<HandsSystem>();
        var handItem = handSystem.GetActiveItem(owner);

        // make sure there is a toggleable item in hand
        if (handItem is null)
            return HTNOperatorStatus.Failed;

        if (!_entManager.TryGetComponent<ItemToggleComponent>(handItem, out var _))
            return HTNOperatorStatus.Failed;

        var toggleSystem = _entManager.System<ItemToggleSystem>();

        // return if already at the desired state
        // TrySetActive down below will return false if we try to turn on something
        // that is already on and vice versa
        if (toggleSystem.IsActivated(handItem.Value) == Toggle)
            return HTNOperatorStatus.Finished;

        toggleSystem.TrySetActive(handItem.Value, Toggle, owner);

        return HTNOperatorStatus.Finished;
    }
}
