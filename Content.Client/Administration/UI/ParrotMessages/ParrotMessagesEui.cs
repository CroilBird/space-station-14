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
    }

    public override void HandleState(EuiStateBase state)
    {
        base.HandleState(state);

        if (state is not ParrotMessagesEuiState { } parrotState)
            return;

        ParrotMessageWindow.UpdateMessages(parrotState);
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
}
