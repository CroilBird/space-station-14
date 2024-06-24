using Content.Shared.Disposal;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Disposal.UI
{
    /// <summary>
    /// Client-side UI used to control a <see cref="MailingUnitComponent"/>
    /// </summary>
    [GenerateTypedNameReferences]
    public sealed partial class MailingUnitWindow : DefaultWindow
    {
        public MailingUnitWindow()
        {
            RobustXamlLoader.Load(this);
        }

        /// <summary>
        /// Update the interface state for the disposals window.
        /// </summary>
        /// <returns>true if we should stop updating every frame.</returns>
        public bool UpdateState(MailingUnitBoundUserInterfaceState state)
        {
            // mailing stuff first
            Title = Loc.GetString("ui-mailing-unit-window-title", ("tag", state.Tag ?? " "));
            //UnitTag.Text = state.Tag;
            Target.Text = state.Target;

            TargetListContainer.Clear();
            foreach (var target in state.TargetList)
            {
                TargetListContainer.AddItem(target);
            }

            // optionally disposal state if it's there
            // this won't be here if we're forcing an update when something on the mail
            // bits above changes
            if (state.DisposalState == null)
                return false;

            var disposalState = state.DisposalState;

            UnitState.Text = disposalState.UnitState;
            var pressureReached = PressureBar.UpdatePressure(disposalState.FullPressureTime);
            Power.Pressed = disposalState.Powered;
            Engage.Pressed = disposalState.Engaged;


            return !disposalState.Powered || pressureReached;
        }

        public bool UpdatePressure(TimeSpan stateFullPressureTime)
        {
            return PressureBar.UpdatePressure(stateFullPressureTime);
        }
    }
}
