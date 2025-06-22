using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.ParrotMessages;

[Serializable, NetSerializable]
public sealed class ParrotMessagesEuiState : EuiStateBase
{
    public ParrotMessagesEuiState(bool showBlocked, bool showOld, List<SharedParrotMessage> messages)
    {
        ShowBlocked = showBlocked;
        ShowOld = showOld;
        Messages = messages;
    }

    public bool ShowBlocked { get; }
    public bool ShowOld { get; }
    public List<SharedParrotMessage> Messages { get; }
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
    public ParrotMessageFilterChangeMsg(bool showBlocked, bool showOld, string filterString)
    {
        ShowBlocked = showBlocked;
        ShowOld = showOld;
        FilterString = filterString;
    }

    public bool ShowBlocked { get; }
    public bool ShowOld { get; }
    public string FilterString { get; }
}
