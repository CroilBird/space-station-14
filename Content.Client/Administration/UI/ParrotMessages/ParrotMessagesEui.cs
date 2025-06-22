using Content.Client.Eui;
using Content.Shared.Administration.ParrotMessages;
using Content.Shared.Eui;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.ParrotMessages;

[UsedImplicitly]
public sealed class ParrotMessagesEui : BaseEui
{
    private ParrotMessagesWindow ParrotMessageWindow { get; }

    public ParrotMessagesEui()
    {
        ParrotMessageWindow = new ParrotMessagesWindow();

        ParrotMessageWindow.OnOpen += () => SendMessage(new ParrotMessageRefreshMsg());
        ParrotMessageWindow.OnClose += () => SendMessage(new CloseEuiMessage());

        ParrotMessageWindow.ParrotMessageRefreshButton.OnPressed += OnRequestRefresh;
        ParrotMessageWindow.ParrotMessageHideBlockCheck.OnToggled += OnBlockCheckToggle;
        ParrotMessageWindow.ParrotMessageFilterOldCheck.OnToggled += OnFilterOldCheckToggle;
    }

    private void OnFilterOldCheckToggle(BaseButton.ButtonToggledEventArgs obj)
    {
        SendFilterChangeMsg();
    }

    private void OnBlockCheckToggle(BaseButton.ButtonEventArgs obj)
    {
        SendFilterChangeMsg();
    }

    private void OnRequestRefresh(BaseButton.ButtonEventArgs obj)
    {
        SendRefreshRequest();
    }

    public override void Opened()
    {
        ParrotMessageWindow.OpenCentered();
    }

    public override void Closed()
    {
        base.Closed();
        ParrotMessageWindow.Close();
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is not ParrotMessagesEuiState { } parrotState)
            return;

        ParrotMessageWindow.UpdateMessageCountText(parrotState.Messages.Count);

        ParrotMessageWindow.ParrotMessageContainer.RemoveAllChildren();

        foreach (var message in parrotState.Messages)
        {
            var messageLine = ParrotMessageWindow.AddMessageLine(message);

            if (message.Blocked)
            {
                messageLine.ParrotBlockButton.Text = Loc.GetString("parrot-messages-line-unblock");
                messageLine.ParrotBlockButton.ToolTip = Loc.GetString("parrot-messages-line-unblock-tooltip");
                messageLine.ParrotBlockButton.OnPressed += _ =>
                {
                    SendUnblockRequest(message.MessageId);
                };
                continue;
            }

            messageLine.ParrotBlockButton.OnPressed += _ =>
            {
                SendBlockRequest(message.MessageId);
            };
        }
    }

    private void SendRefreshRequest()
    {
        SendMessage(new ParrotMessageRefreshMsg());
    }

    private void SendBlockRequest(int messageId)
    {
        SendMessage(new ParrotMessageBlockChangeMsg(messageId, true));
    }

    private void SendUnblockRequest(int messageId)
    {
        SendMessage(new ParrotMessageBlockChangeMsg(messageId, false));
    }

    private void SendFilterChangeMsg()
    {
        var showBlocked = ParrotMessageWindow.ParrotMessageHideBlockCheck.Pressed;
        var showOld = ParrotMessageWindow.ParrotMessageFilterOldCheck.Pressed;
        var filterString = ParrotMessageWindow.ParrotFilterLineEdit.Text;

        var msg = new ParrotMessageFilterChangeMsg(showBlocked, showOld, filterString);

        SendMessage(msg);
    }
}
