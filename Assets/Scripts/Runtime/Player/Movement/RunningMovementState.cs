using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RunningMovementState : MovementState
{


    float standingStillTimer;

    public RunningMovementState(PlayerMovement movement, EntityPhysics physics, PlayerAnimationController anim, MovementSettings settings)
        : base(movement, physics, anim, settings)
    {
        Type = MovementStateType.Running;
    }

    public override bool AcceptMovementInput()
    {
        return true;
    }

    public override void Update()
    {
        if (!(Input.GetKey(KeyCode.LeftArrow) ^ Input.GetKey(KeyCode.RightArrow)))
        {
            standingStillTimer += Time.deltaTime;

            if(standingStillTimer > 0.3f)
            {
                Movement.SetState(MovementStateType.Base);
            }
        }
        else
        {
            standingStillTimer = 0;
        }

    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        Anim.ActivateTrigger(PlayerTriggerType.Run);
        Anim.SetFlag(PlayerFlagType.Running, true);
    }

    public override void OnExitState()
    {
        base.OnExitState();
        Anim.SetFlag(PlayerFlagType.Running, false);
    }
}
