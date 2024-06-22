using Content.Client.Disposal.Systems;
using Content.Shared.Disposal;
using Content.Shared.Disposal.Components;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using static Content.Shared.Disposal.Components.SharedDisposalUnitComponent;

namespace Content.Client.Disposal.UI
{
    /// <summary>
    /// Initializes a <see cref="MailingUnitWindow"/> and updates it when new server messages are received.
    /// </summary>
    [UsedImplicitly]
    public sealed class MailingUnitBoundInterface : BoundUserInterface
    {
        [ViewVariables]
        private MailingUnitWindow? _window;

        public MailingUnitBoundInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        private void ButtonPressed(UiButton button)
        {
            SendMessage(new UiButtonPressedMessage(button));
            // If we get client-side power stuff then we can predict the button presses but for now we won't as it stuffs
            // the pressure lerp up.
        }

        private void TargetSelected(ItemList.ItemListSelectedEventArgs args)
        {
            var item = args.ItemList[args.ItemIndex];
            SendMessage(new TargetSelectedMessage(item.Text));
        }

        protected override void Open()
        {
            base.Open();

            _window = new MailingUnitWindow();

            _window.OpenCenteredRight();
            _window.OnClose += Close;

            _window.Eject.OnPressed += _ => ButtonPressed(UiButton.Eject);
            _window.Engage.OnPressed += _ => ButtonPressed(UiButton.Engage);
            _window.Power.OnPressed += _ => ButtonPressed(UiButton.Power);

            _window.TargetListContainer.OnItemSelected += TargetSelected;
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);

            if (state is not MailingUnitBoundUserInterfaceState cast)
                return;

            _window?.UpdateState(cast);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            _window?.Dispose();
        }
    }
}
