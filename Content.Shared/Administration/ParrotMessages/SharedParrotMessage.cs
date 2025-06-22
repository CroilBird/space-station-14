using Robust.Shared.Serialization;

namespace Content.Shared.Administration.ParrotMessages;

[Serializable, NetSerializable]
public sealed record SharedParrotMessage(
    int MessageId,
    string MessageText,
    int SourceRound,
    string SourcePlayerUserName,
    Guid SourcePlayerGuid,
    bool Blocked
);
