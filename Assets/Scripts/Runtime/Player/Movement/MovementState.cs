using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum MovementStateType
{
    Base,
    Ducking,
    Running,
    ENUM_COUNT,
}

public struct MovementSettings
{
    public float MaxHorizontalSpeed;
    public float MaxVerticalSpeed;
}

public class MovementState
{
    public MovementStateType Type { get; protected set; }

    protected EntityPhysics Physics;
    protected PlayerAnimationController Anim;
    protected MovementSettings Settings;
    protected PlayerMovement Movement;

    public MovementState(PlayerMovement movement, EntityPhysics physics, PlayerAnimationController anim, MovementSettings settings)
    {
        Type = MovementStateType.Base;
        Movement = movement;
        Physics = physics;
        Anim = anim;
        Settings = settings;
    }

    public virtual bool AcceptMovementInput()
    {
        return true;
    }

    public virtual bool CanEnterState()
    {
        return true;
    }

    public virtual void Update()
    {

    }

    public virtual void OnEnterState()
    {
        Physics.CurrentMaxHorizontalSpeed = Settings.MaxHorizontalSpeed;
        Physics.MaxVerticalSpeed = Settings.MaxVerticalSpeed;
    }

    public virtual void OnExitState()
    {

    }
}
