using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float MovementAcceleration;
    public float JumpStrength;


    private PlayerAnimationController animController;
    private EntityPhysics physics;

	void Start ()
	{
	    physics = GetComponent<EntityPhysics>();
	    animController = GetComponent<PlayerAnimationController>();
	}
	
	void Update () 
    {
	    UpdateMovement();

	    if (Input.GetKeyDown(KeyCode.D))
	    {
	        animController.ActivateTrigger(PlayerTriggerType.Stab);
	    }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animController.ActivateTrigger(PlayerTriggerType.Kick);
        }
	}

    void UpdateMovement()
    {
        physics.HorizontalMoveAmount = 0;
        if (true)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                physics.HorizontalMoveAmount -= MovementAcceleration;
                animController.SetFacing(1);

            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                physics.HorizontalMoveAmount += MovementAcceleration;
                animController.SetFacing(-1);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            bool didDoubleJump;
            if (physics.Jump(JumpStrength, true, out didDoubleJump))
            {
                animController.ActivateTrigger(PlayerTriggerType.Jump);
            }
        }
    }
}
