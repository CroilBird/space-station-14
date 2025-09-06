using Content.Shared.Magic.Components;
using Content.Shared.Physics;
using Robust.Shared.Containers;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using System.Linq;
using Content.Shared.Damage;
using Content.Shared.Magic.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared.Magic.Systems;

public sealed class AnimateSpellSystem : EntitySystem
{
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<AnimateComponent, MapInitEvent>(OnAnimate);
        SubscribeLocalEvent<AnimateComponent, GetMeleeDamageEvent>(OnGetMeleeDamage);
        SubscribeLocalEvent<AnimateComponent, RefreshMovementSpeedModifiersEvent>(OnMoveSpeedmodified);
        SubscribeLocalEvent<AnimateComponent, AfterChangeComponentSpellEvent>(AfterComponentChangeSpell);
    }

    private void OnGetMeleeDamage(Entity<AnimateComponent> ent, ref GetMeleeDamageEvent args)
    {
        if (!TryComp<MeleeWeaponComponent>(ent, out var melee))
            return;

        args.Damage = GetAnimatedEntityDamage((ent.Owner, ent.Comp, melee));
    }

    private void OnMoveSpeedmodified(Entity<AnimateComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        if (!TryComp<FixturesComponent>(ent, out var fixtures))
            return;

        var newSpeed = GetAnimatedEntitySpeed((ent.Owner, ent.Comp, fixtures));

        args.ModifySpeed(newSpeed);
    }

    private void AfterComponentChangeSpell(Entity<AnimateComponent> ent, ref AfterChangeComponentSpellEvent args)
    {
        _movementSpeed.RefreshMovementSpeedModifiers(ent);

        // really should not happen. There is no fallback for this
        if (!TryComp<DamageableComponent>(ent, out var damage))
            return;

        // TODO: set new health/damage thresholds
    }

    private DamageSpecifier GetAnimatedEntityDamage(Entity<AnimateComponent, MeleeWeaponComponent> ent)
    {
        // just use the damage from the meleeweapon component if this is a special animated entity
        // e.g. captain's sword, fire axe and stuff like that
        if (!ent.Comp1.UseCalculatedDamage)
            return ent.Comp2.Damage;

        // from this point we will try to calculate a damage value for this animated entity

        // start with the damage at the fallback damage. If we can't calculate a sensible damage value, we will use this
        var calculatedDamage = ent.Comp1.FallbackDamage;

        // get fixture density. If there is no fixture component for whatever reason, don't do damage at all!!
        // This should never ever happen, but we do this so that people don't get killed by stuff they can't destroy
        if (!TryComp<FixturesComponent>(ent, out var fixtures))
            return calculatedDamage;

        // same thing, but with having a fixture component without fixtures
        if (fixtures.FixtureCount == 0)
            return calculatedDamage;

        // now we have to guess which fixture to use for damage calculation
        // use the first
        var density = fixtures.Fixtures.First().Value.Density;
        var newBluntDamage = density * ent.Comp1.FixtureDensityDamageMultiplier;

        calculatedDamage.DamageDict["Blunt"] = newBluntDamage;

        calculatedDamage.Clamp(ent.Comp1.MinimumDamage, ent.Comp1.MaximumDamage);

        return calculatedDamage;
    }

    private float GetAnimatedEntitySpeed(Entity<AnimateComponent, FixturesComponent> ent)
    {
        // we have to calculate the speed. If we fail to get a density, we should err on the side of caution and use a
        // slow base movement speed
        var newSpeed = ent.Comp1.MinimumSpeed;

        // if we don't have fixture on our fixture, use minimum speed
        if (ent.Comp2.FixtureCount == 0)
            return newSpeed;

        var density = ent.Comp2.Fixtures.First().Value.Density;

        newSpeed = density * ent.Comp1.FixtureDensitySpeedMultiplier + ent.Comp1.FixtureDensitiySpeedBase;

        newSpeed = Math.Clamp(newSpeed, ent.Comp1.MinimumSpeed, ent.Comp1.MaximumSpeed);

        return newSpeed;
    }

    private void OnAnimate(Entity<AnimateComponent> ent, ref MapInitEvent args)
    {
        // Physics bullshittery necessary for object to behave properly

        if (!TryComp<FixturesComponent>(ent, out var fixtures) || !TryComp<PhysicsComponent>(ent, out var physics))
            return;

        var xform = Transform(ent);
        var fixture = fixtures.Fixtures.First();

        _transform.Unanchor(ent); // If left anchored they are effectively stuck/immobile and not a threat
        _physics.SetCanCollide(ent, true, true, false, fixtures, physics);
        _physics.SetCollisionMask(ent, fixture.Key, fixture.Value, (int)CollisionGroup.FlyingMobMask, fixtures, physics);
        _physics.SetCollisionLayer(ent, fixture.Key, fixture.Value, (int)CollisionGroup.FlyingMobLayer, fixtures, physics);
        _physics.SetBodyType(ent, BodyType.KinematicController, fixtures, physics, xform);
        _physics.SetBodyStatus(ent, physics, BodyStatus.InAir, true);
        _physics.SetFixedRotation(ent, false, true, fixtures, physics);
        _physics.SetHard(ent, fixture.Value, true, fixtures);
        _container.AttachParentToContainerOrGrid((ent, xform)); // Items animated inside inventory now exit, they can't be picked up and so can't escape otherwise

        var ev = new AnimateSpellEvent();
        RaiseLocalEvent(ent, ref ev);
    }
}

[ByRefEvent]
public readonly record struct AnimateSpellEvent;
