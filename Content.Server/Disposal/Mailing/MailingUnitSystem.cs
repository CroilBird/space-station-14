using Content.Server.Configurable;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Disposal.Unit.EntitySystems;
using Content.Server.Power.Components;
using Content.Shared.DeviceNetwork;
using Content.Shared.Disposal;
using Content.Shared.Interaction;
using Robust.Server.GameObjects;
using Robust.Shared.Player;

namespace Content.Server.Disposal.Mailing;

public sealed class MailingUnitSystem : EntitySystem
{
    [Dependency] private readonly DeviceNetworkSystem _deviceNetworkSystem = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterfaceSystem = default!;

    private const string MailTag = "mail";

    private const string TagConfigurationKey = "tag";

    private const string NetTag = "tag";
    private const string NetSrc = "src";
    private const string NetTarget = "target";
    private const string NetCmdSent = "mail_sent";
    private const string NetCmdRequest = "get_mailer_tag";
    private const string NetCmdResponse = "mailer_tag";
    private const string NetCmdUpdate = "tag_change";
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MailingUnitComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<MailingUnitComponent, DeviceNetworkPacketEvent>(OnPacketReceived);
        SubscribeLocalEvent<MailingUnitComponent, BeforeDisposalFlushEvent>(OnBeforeFlush);
        SubscribeLocalEvent<MailingUnitComponent, ConfigurationSystem.ConfigurationUpdatedEvent>(OnConfigurationUpdated);
        SubscribeLocalEvent<MailingUnitComponent, DisposalUnitUIStateUpdatedEvent>(OnDisposalUnitUIStateChange);
        SubscribeLocalEvent<MailingUnitComponent, TargetSelectedMessage>(OnTargetSelected);
        SubscribeLocalEvent<MailingUnitComponent, PowerChangedEvent>(OnPowerChanged);
    }


    private void OnComponentInit(EntityUid uid, MailingUnitComponent component, ComponentInit args)
    {
        UpdateTargetList(uid, component);
    }

    private void OnPacketReceived(EntityUid uid, MailingUnitComponent component, DeviceNetworkPacketEvent args)
    {
        if (!args.Data.TryGetValue(DeviceNetworkConstants.Command, out string? command) || !IsPowered(uid))
            return;

        switch (command)
        {
            case NetCmdRequest:
                SendTagRequestResponse(uid, args, component.Tag);
                break;
            case NetCmdResponse when args.Data.TryGetValue(NetTag, out string? tag):
                // Add the received tag request response to the list of targets if not already there
                if (!component.TargetList.Contains(tag))
                    component.TargetList.Add(tag);
                UpdateUserInterface(uid, component);
                break;
            case NetCmdUpdate:
                UpdateTag(uid, component, args.Data);
                break;
        }
    }

    /// <summary>
    /// Sends the given tag as a response to a <see cref="NetCmdRequest"/> if it's not null
    /// </summary>
    private void SendTagRequestResponse(EntityUid uid, DeviceNetworkPacketEvent args, string? tag)
    {
        if (tag == null)
            return;

        var payload = new NetworkPayload
        {
            [DeviceNetworkConstants.Command] = NetCmdResponse,
            [NetTag] = tag
        };

        _deviceNetworkSystem.QueuePacket(uid, args.Address, payload, args.Frequency);
    }

    /// <summary>
    /// Prevents the unit from flushing if no target is selected
    /// </summary>
    private void OnBeforeFlush(EntityUid uid, MailingUnitComponent component, BeforeDisposalFlushEvent args)
    {
        if (string.IsNullOrEmpty(component.Target))
        {
            args.Cancel();
            return;
        }

        args.Tags.Add(MailTag);
        args.Tags.Add(component.Target);

        BroadcastSentMessage(uid, component);
    }

    /// <summary>
    /// Broadcast that a mail was sent including the src and target tags
    /// </summary>
    private void BroadcastSentMessage(EntityUid uid, MailingUnitComponent component, DeviceNetworkComponent? device = null)
    {
        if (string.IsNullOrEmpty(component.Tag) || string.IsNullOrEmpty(component.Target) || !Resolve(uid, ref device))
            return;

        var payload = new NetworkPayload
        {
            [DeviceNetworkConstants.Command] = NetCmdSent,
            [NetSrc] = component.Tag,
            [NetTarget] = component.Target
        };

        _deviceNetworkSystem.QueuePacket(uid, null, payload, null, null, device);
    }

    /// <summary>
    /// Clears the units target list and broadcasts a <see cref="NetCmdRequest"/>.
    /// The target list will then get populated with <see cref="NetCmdResponse"/> responses from all active mailing units on the same grid
    /// </summary>
    private void UpdateTargetList(EntityUid uid, MailingUnitComponent component, DeviceNetworkComponent? device = null)
    {
        if (!Resolve(uid, ref device, false))
            return;

        var payload = new NetworkPayload
        {
            [DeviceNetworkConstants.Command] = NetCmdRequest
        };

        component.TargetList.Clear();
        _deviceNetworkSystem.QueuePacket(uid, null, payload, null, null, device);
    }

    private void SendTagUpdate(EntityUid uid, MailingUnitComponent component, string oldTag, string newTag, DeviceNetworkComponent? device = null)
    {
        if (!Resolve(uid, ref device, false))
            return;

        var payload = new NetworkPayload
        {
            [DeviceNetworkConstants.Command] = NetCmdUpdate,
            [NetSrc] = oldTag,
            [NetTag] = newTag
        };

        _deviceNetworkSystem.QueuePacket(uid, null, payload, null, null, device);
    }

    private void UpdateTag(EntityUid uid, MailingUnitComponent component, NetworkPayload data)
    {
        if (!data.TryGetValue<string>(NetSrc, out var oldTag) || !data.TryGetValue<string>(NetTag, out var newTag))
            return;

        component.TargetList.Remove(oldTag);
        component.TargetList.Add(newTag);
    }

    /// <summary>
    /// Gets called when the units tag got updated
    /// </summary>
    private void OnConfigurationUpdated(EntityUid uid, MailingUnitComponent component, ConfigurationSystem.ConfigurationUpdatedEvent args)
    {
        var configuration = args.Configuration.Config;
        if (!configuration.ContainsKey(TagConfigurationKey) || configuration[TagConfigurationKey] == string.Empty)
        {
            component.Tag = null;
            return;
        }

        var oldTag = component.Tag;
        var newTag = configuration[TagConfigurationKey];

        // this isn't great. handling of a null tag is all over the place in this component.
        // to be fixed in rework
        if (oldTag == null || newTag == null)
            return;

        SendTagUpdate(uid, component, oldTag, newTag);

        component.Tag = newTag;
        UpdateUserInterface(uid, component);
    }


    /// <summary>
    /// Gets called when the disposal unit components ui state changes. This is required because the mailing unit requires a disposal unit component and overrides its ui
    /// </summary>
    private void OnDisposalUnitUIStateChange(EntityUid uid, MailingUnitComponent component, DisposalUnitUIStateUpdatedEvent args)
    {
        component.DisposalUnitInterfaceState = args.State;
        UpdateUserInterface(uid, component);
    }

    private void UpdateUserInterface(EntityUid uid, MailingUnitComponent component)
    {
        var state = new MailingUnitBoundUserInterfaceState(component.DisposalUnitInterfaceState, component.Target, component.TargetList, component.Tag);
        _userInterfaceSystem.SetUiState(uid, MailingUnitUiKey.Key, state);
    }

    private void OnTargetSelected(EntityUid uid, MailingUnitComponent component, TargetSelectedMessage args)
    {
        component.Target = args.Target;
        UpdateUserInterface(uid, component);
    }

    /// <summary>
    /// Checks if the unit is powered if an <see cref="ApcPowerReceiverComponent"/> is present
    /// </summary>
    /// <returns>True if the power receiver component is powered or not present</returns>
    private bool IsPowered(EntityUid uid, ApcPowerReceiverComponent? powerReceiver = null)
    {
        if (Resolve(uid, ref powerReceiver) && !powerReceiver.Powered)
            return false;

        return true;
    }

    private void OnPowerChanged(EntityUid uid, MailingUnitComponent component, PowerChangedEvent args)
    {
        // if we are powering on, send a request for tags
        // this is so that someone turn it on and off again when something is off. very realistic
        if (args.Powered)
            UpdateTargetList(uid, component);
    }
}
