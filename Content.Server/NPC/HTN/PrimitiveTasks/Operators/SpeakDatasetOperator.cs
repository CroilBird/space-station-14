using Content.Server.Chat.Systems;
using Content.Shared.Chat;
using Content.Shared.Dataset;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators;

/// <summary>
/// Makes this entity speak from a specified dataset
/// </summary>
public sealed partial class SpeakDatasetOperator : HTNOperator
{
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private ChatSystem _chat = default!;

    /// <summary>
    /// The dataset to select from. Strings will be picked at random
    /// </summary>
    [DataField(required: true)]
    public ProtoId<LocalizedDatasetPrototype> Dataset;

    public override void Initialize(IEntitySystemManager sysManager)
    {
        base.Initialize(sysManager);

        _chat = sysManager.GetEntitySystem<ChatSystem>();
    }

    public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
    {
        var speaker = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        var dataset = _protoMan.Index(Dataset);

        string text = _random.Pick(dataset);

        _chat.TrySendInGameICMessage(speaker, Loc.GetString(text), InGameICChatType.Speak, false);

        return base.Update(blackboard, frameTime);
    }
}
