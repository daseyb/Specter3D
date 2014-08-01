using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DashingMovementState : MovementState
{
    float dashTimer = 0;
    int startDir = 0;

    public DashingMovementState(PlayerMovement movement, EntityPhysics physics, PlayerAnimationController anim, MovementSettings settings)
        : base(movement, physics, anim, settings)
    {
        Type = MovementStateType.Dashing;
    }

    public override bool AcceptMovementInput()
    {
        return false;
    }

    public override void Update()
    {
        base.Update();
        dashTimer += Time.deltaTime;

        if (dashTimer > 0.3f)
        {
            Movement.SetState(MovementStateType.Base);
        }

        Physics.HorizontalMoveAmount = startDir * 3000;
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        startDir = Anim.GetFacing();
        dashTimer = 0;
        Anim.SetFlag(PlayerFlagType.Dashing, true);
        Anim.ActivateTrigger(PlayerTriggerType.Dash);
    }

    public override void OnExitState()
    {
        base.OnExitState();
        Anim.SetFlag(PlayerFlagType.Dashing, false);
    }
}
