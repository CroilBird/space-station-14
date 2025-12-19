using Content.Shared.Contraband;
using Content.Shared.Emag.Systems;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;

namespace Content.Shared.Silicons.Bots.SecBot;

public sealed class SecurityThreatSystem : EntitySystem
{
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly ContrabandSystem _contraband = default!;
    [Dependency] private readonly EmagSystem _emag = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PotentialSecurityThreatComponent, DidEquipHandEvent>(OnDidEquipHand);
        SubscribeLocalEvent<PotentialSecurityThreatComponent, DidUnequipHandEvent>(OnDidUnequipHand);

        SubscribeLocalEvent<SecurityThreatSeekerComponent, GotEmaggedEvent>(Handler);
    }

    private void Handler(Entity<SecurityThreatSeekerComponent> ent, ref GotEmaggedEvent args)
    {
        if (!_emag.CompareFlag(args.Type, EmagType.Interaction))
            return;

        if (_emag.CheckFlag(ent, EmagType.Interaction))
            return;

        args.Handled = true;
    }

    private void OnDidEquipHand(Entity<PotentialSecurityThreatComponent> potentialThreat, ref DidEquipHandEvent args)
    {
        UpdateThreat(potentialThreat);
    }

    private void OnDidUnequipHand(Entity<PotentialSecurityThreatComponent> potentialThreat, ref DidUnequipHandEvent args)
    {
        UpdateThreat(potentialThreat);
    }

    /// <summary>
    /// Returns true if a potential threat is an active threat, which is the case when it exceeds the threat seeker's
    /// threat threshold.
    /// </summary>
    public bool IsActiveThreat(Entity<SecurityThreatSeekerComponent?> threatSeeker,
        Entity<PotentialSecurityThreatComponent?> potentialThreat)
    {
        if (!Resolve(threatSeeker, ref threatSeeker.Comp) || !Resolve(potentialThreat, ref potentialThreat.Comp))
            return false;

        return potentialThreat.Comp.ThreatLevel >= threatSeeker.Comp.ThreatThreshold;
    }

    private void UpdateThreat(Entity<PotentialSecurityThreatComponent> potentialThreat)
    {
        var newThreat = 0;

        newThreat += GetContrabandThreat(potentialThreat);

        potentialThreat.Comp.ThreatLevel = newThreat;
    }

    private int GetContrabandThreat(Entity<PotentialSecurityThreatComponent> potentialThreat)
    {
        if (!HasComp<HandsComponent>(potentialThreat))
            return 0;

        var sum = 0;

        // enumerate over items in hand
        foreach (var item in _hands.EnumerateHeld(potentialThreat.Owner))
        {
            if (!TryComp<ContrabandComponent>(item, out var contrabandComponent))
                continue;

            if (_contraband.UserCanCarryItem(potentialThreat.Owner, contrabandComponent))
                continue;

            // at this point the item is not legal for the user to carry

            // only increase the threat level if the category of the contraband is tracked in thePotentialSecurityThreatComponent
            if (potentialThreat.Comp.ContrabandThreatModifiers.TryGetValue(contrabandComponent.Severity.Id, out var modifier))
                sum += modifier;

        }

        return sum;
    }
}
