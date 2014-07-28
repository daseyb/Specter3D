using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float MovementAcceleration;
    public float JumpStrength;
    public float FlutteringAcceleration;
    public float FlutteringStartTime;
    public float FlutteringDuration;

    private Dictionary<MovementStateType, MovementState> movementStates = new Dictionary<MovementStateType, MovementState>();
    private InputHelper inputHelper = new InputHelper();

    private PlayerAnimationController animController;
    private EntityPhysics physics;

    private MovementState currentState;

    private float jumpKeyTimer = 0;

    private bool fluttering = false;

    private void Start()
	{
	    physics = GetComponent<EntityPhysics>();
	    animController = GetComponent<PlayerAnimationController>();

        InitStates();
	}

    private void OnEnable()
    {
        inputHelper.RegisterDoubleTap(KeyCode.LeftArrow, OnDirectionDoubleTap, 0.3f);
        inputHelper.RegisterDoubleTap(KeyCode.RightArrow, OnDirectionDoubleTap, 0.3f);
    }

    private void OnDisable()
    {
        inputHelper.UnregisterDoubleTap(KeyCode.LeftArrow, OnDirectionDoubleTap);
        inputHelper.UnregisterDoubleTap(KeyCode.RightArrow, OnDirectionDoubleTap);
    }

    private void Update() 
    {
	    UpdateMovement();
        currentState.Update();
        inputHelper.Update();
	}

    private void InitStates()
    {
        movementStates[MovementStateType.Base] = new MovementState(this, physics, animController, new MovementSettings() { MaxHorizontalSpeed = physics.MaxHorizontalSpeed, MaxVerticalSpeed = -1 });
        movementStates[MovementStateType.Ducking] = new DuckingMovementState(this, physics, animController, new MovementSettings() { MaxHorizontalSpeed = 20, MaxVerticalSpeed = -1 });
        movementStates[MovementStateType.Running] = new RunningMovementState(this, physics, animController, new MovementSettings() { MaxHorizontalSpeed = 40, MaxVerticalSpeed = -1 });

        SetState(MovementStateType.Base);
    }

    public void SetState(MovementStateType type)
    {
        var newState = movementStates[type];

        if(newState == currentState || !newState.CanEnterState())
        {
            return;
        }

        if(currentState != null)
        {
            currentState.OnExitState();
        }

        currentState = newState;
        currentState.OnEnterState();
    }

    private void UpdateMovement()
    {
        physics.HorizontalMoveAmount = 0;
        if (currentState.AcceptMovementInput())
        {
            int dir = 0;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                dir--;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                dir++;
            }

            physics.HorizontalMoveAmount = dir * MovementAcceleration;
            animController.SetFacing(-dir);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            bool didDoubleJump;
            if (physics.Jump( currentState.Type == MovementStateType.Running ? JumpStrength * 1.2f : JumpStrength, true, out didDoubleJump))
            {
                animController.ActivateTrigger(didDoubleJump ? PlayerTriggerType.DoubleJump : PlayerTriggerType.Jump);
                if(didDoubleJump)
                {
                    jumpKeyTimer = 0;
                }

                //SetState(MovementStateType.Base);
            }

        }

        if(Input.GetKeyUp(KeyCode.S))
        {
            jumpKeyTimer = FlutteringStartTime + FlutteringDuration + 1;
            physics.EndJump();
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetState(MovementStateType.Ducking);
        }

        if(currentState.Type == MovementStateType.Ducking && Input.GetKeyUp(KeyCode.DownArrow))
        {
            SetState(MovementStateType.Base);
        }

        /*if (currentState.Type == MovementStateType.Running)
        {
            SetState(MovementStateType.Base);
        }*/

        UpdateFluttering();
    }

    private void UpdateFluttering()
    {
        if (fluttering || (Input.GetKey(KeyCode.S) && !physics.IsGrounded && physics.Velocity.y < 0 && physics.DidDoubleJump))
        {
            jumpKeyTimer += Time.deltaTime;
        }
        else if (physics.IsGrounded)
        {
            jumpKeyTimer = 0;
        }

        bool prevFluttering = fluttering;
        fluttering = jumpKeyTimer > FlutteringStartTime && jumpKeyTimer < FlutteringStartTime + FlutteringDuration;
        animController.SetFlag(PlayerFlagType.Fluttering, fluttering);
        physics.VerticalMoveAmount = fluttering ? FlutteringAcceleration : 0;
    }

    private void OnDirectionDoubleTap()
    {
        SetState(MovementStateType.Running);
    }
}
