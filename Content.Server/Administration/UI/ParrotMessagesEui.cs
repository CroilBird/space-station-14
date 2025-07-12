using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Administration.ParrotMessages;
using Content.Shared.Eui;

namespace Content.Server.Administration.UI;

public sealed class ParrotMessagesEui : BaseEui
{
    [Dependency] private readonly IAdminManager _adminManager = default!;
    [Dependency] private readonly IServerDbManager _db = default!;

    public ParrotMessagesEui()
    {
        IoCManager.InjectDependencies(this);
    }

    private string _filterString = string.Empty;
    private readonly List<ExtendedParrotMemory> _parrotMessages = [];

    public override EuiStateBase GetNewState()
    {
        return new ParrotMessagesEuiState(_parrotMessages);
    }

    public override async void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (!_adminManager.HasAdminFlag(Player, AdminFlags.Moderator))
            return;

        switch (msg)
        {
            case ParrotMessageRefreshMsg:
                RefreshParrotMessages();

                break;
            case ParrotMessageBlockChangeMsg blockChangeMsg:
                SetParrotMessageBlock(blockChangeMsg.MessageId, blockChangeMsg.Block);
                break;

            case ParrotMessageFilterChangeMsg filterChangeMsg:
                _filterString = filterChangeMsg.FilterString;
                RefreshParrotMessages();
                break;
        }
    }

    private async void SetParrotMessageBlock(int messageId, bool block)
    {
        await _db.SetParrotMemoryBlock(messageId, block);
    }

    private async void RefreshParrotMessages()
    {
        var messages = _db.GetParrotMemories(false);

        _parrotMessages.Clear();

        await foreach (var message in messages)
        {
            _parrotMessages.Add(message);
        }

        StateDirty();
    }
}
