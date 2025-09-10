using Content.Shared.Magic.Components;
using Content.Shared.Physics;
using Robust.Shared.Containers;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using System.Linq;
using Content.Shared.Damage;
using Content.Shared.Item;
using Content.Shared.Magic.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Storage;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Prototypes;

namespace Content.Shared.Magic.Systems;

public abstract class SharedAnimateSpellSystem : EntitySystem
{
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedItemSystem _item = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private static readonly ProtoId<ItemSizePrototype> GinormousSize = "Ginormous";

    public override void Initialize()
    {
        SubscribeLocalEvent<AnimateableComponent, MapInitEvent>(OnAnimateableMapInit);

        SubscribeLocalEvent<AnimateComponent, MapInitEvent>(OnAnimate);
        SubscribeLocalEvent<AnimateComponent, AfterChangeComponentSpellEvent>(AfterComponentChange);
        SubscribeLocalEvent<AnimateComponent, RefreshMovementSpeedModifiersEvent>(OnMoveSpeedModified);
    }

    private void OnAnimateableMapInit(Entity<AnimateableComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.Area = GetAnimatedEntityArea(ent);
    }

    private void AfterComponentChange(Entity<AnimateComponent> ent, ref AfterChangeComponentSpellEvent args)
    {

        // get the movement speed system to raise the event to refresh movement speed modifiers. Will call
        // OnMoveSpeedModified later on
        _movementSpeed.RefreshMovementSpeedModifiers(ent);

        // really should not happen. There is no fallback for this
        if (!TryComp<DamageableComponent>(ent, out var damage))
            return;

        SetAnimatedEntityDamageThreshold(ent);
    }

    private void OnMoveSpeedModified(Entity<AnimateComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        if (!TryComp<AnimateableComponent>(ent, out var animateable))
            return;

        var newSpeed = animateable.BaseSpeed + animateable.Area * animateable.AreaSpeedMultiplier;

        newSpeed = Math.Clamp(newSpeed, animateable.MinimumSpeed, animateable.MaximumSpeed);

        args.ModifySpeed(newSpeed);
    }

    private float GetAnimatedEntityArea(Entity<AnimateableComponent, ItemComponent?> entity)
    {
        // things that can't even be picked up
        if (!Resolve(entity, ref entity.Comp2, false))
            return entity.Comp1.FallbackArea;

        if (_item.GetItemSizeWeight(entity.Comp2.Size) == _item.GetItemSizeWeight(GinormousSize))
            return entity.Comp1.GinormousItemArea;

        var itemArea = _item.GetItemShape(entity.Comp2).GetArea() * entity.Comp1.AreaMultiplier;

        return itemArea;
    }

    protected virtual void SetAnimatedEntityDamageThreshold(Entity<AnimateComponent> entity)
    {

    }

    private void SetAnimatedEntityMeleeDamage(Entity<AnimateableComponent?> entity)
    {
        // if there already is a preset melee component, use the damage from that
        // this ensures plushies and specific weapons remain interesting choices
        if (HasComp<MeleeWeaponComponent>(entity))
            return;

        if (!Resolve(entity, ref entity.Comp))
            return;

        var calculatedDamage = entity.Comp.Area * entity.Comp.AreaDamageMultiplier;

        calculatedDamage = Math.Clamp(calculatedDamage, entity.Comp.MinimumDamage, entity.Comp.MaximumDamage);

        // if we fail to get the prototype here, we quit and just don't allow the entity to have a melee damage component
        // just to be on the safe side
        if (!_prototype.Resolve(entity.Comp.DamageType, out var damagePrototype))
            return;

        // add the new component
        EnsureComp<MeleeWeaponComponent>(entity, out var melee);

        melee.Damage = new DamageSpecifier(damagePrototype, calculatedDamage);
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

        SetAnimatedEntityMeleeDamage(ent.Owner);

        var ev = new AnimateSpellEvent();
        RaiseLocalEvent(ent, ref ev);
    }
}

[ByRefEvent]
public readonly record struct AnimateSpellEvent;
