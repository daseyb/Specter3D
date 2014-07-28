using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DuckingMovementState : MovementState
{
    public DuckingMovementState(PlayerMovement movement, EntityPhysics physics, PlayerAnimationController anim, MovementSettings settings)
        : base(movement, physics, anim, settings)
    {
        Type = MovementStateType.Ducking;
    }

    public override bool AcceptMovementInput()
    {
        return false;
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        Anim.SetFlag(PlayerFlagType.Ducking, true);
    }

    public override void OnExitState()
    {
        base.OnExitState();
        Anim.SetFlag(PlayerFlagType.Ducking, false);
    }
}
