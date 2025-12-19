using Content.Shared.Access.Systems;
using Content.Shared.Contraband;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;

namespace Content.Shared.Silicons.Bots;

public sealed class SecurityThreatSystem : EntitySystem
{
    public int GetThreat(Entity<SecurityThreatComponent?> potentialThreat)
    {
        if (!Resolve(potentialThreat, ref potentialThreat.Comp))
            return 0;

        return 10;
    }

    public bool IsThreat(Entity<SecurityThreatSeekerComponent?> threatSeeker,
        EntityUid potentialThreat)
    {
        if (!Resolve(threatSeeker, ref threatSeeker.Comp))
            return false;

        return GetThreat(potentialThreat) >= threatSeeker.Comp.ThreatThreshold;
    }
}
