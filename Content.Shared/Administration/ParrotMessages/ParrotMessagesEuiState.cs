using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.ParrotMessages;

[Serializable, NetSerializable]
public sealed class ParrotMessagesEuiState : EuiStateBase
{
    public ParrotMessagesEuiState(List<ExtendedParrotMemory> messages)
    {
        Messages = messages;
    }

    public List<ExtendedParrotMemory> Messages { get; }
}

[Serializable, NetSerializable]
public sealed class ParrotMessageRefreshMsg : EuiMessageBase
{

}

[Serializable, NetSerializable]
public sealed class ParrotMessageBlockChangeMsg : EuiMessageBase
{
    public ParrotMessageBlockChangeMsg(int messageId, bool block)
    {
        MessageId = messageId;
        Block = block;
    }

    public int MessageId { get; }
    public bool Block { get; }
}

[Serializable, NetSerializable]
public sealed class ParrotMessageFilterChangeMsg : EuiMessageBase
{
    public ParrotMessageFilterChangeMsg(string filterString)
    {
        FilterString = filterString;
    }

    public string FilterString { get; }
}
