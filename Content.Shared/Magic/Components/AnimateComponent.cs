
using System.ComponentModel.DataAnnotations;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Magic.Components;

// Added to objects when they are made animate
[RegisterComponent, NetworkedComponent]
public sealed partial class AnimateComponent : Component
{

}
