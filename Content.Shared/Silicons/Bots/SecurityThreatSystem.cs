using Content.Shared.Access.Systems;
using Content.Shared.Inventory;

namespace Content.Shared.Silicons.Bots;

public sealed class SecurityThreatSystem : EntitySystem
{
    [Dependency] private readonly SharedAccessSystem _access = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;


    public override void Initialize()
    {
        base.Initialize();


    }


    public bool IsThreat(Entity<SecurityThreatSeekerComponent?> threatSeeker,
        Entity<SecurityThreatComponent?> potentialThreat)
    {
        if (!Resolve(threatSeeker.Owner, ref threatSeeker.Comp) || !Resolve(potentialThreat.Owner, ref potentialThreat.Comp))
            return false;

        return potentialThreat.Comp.CurrentThreat >= threatSeeker.Comp.ThreatThreshold;
    }
}
