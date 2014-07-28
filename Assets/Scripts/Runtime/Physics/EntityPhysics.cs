using UnityEngine;
using System.Collections;
using System;

public class EntityPhysics : MonoBehaviour {

	public GameObject FeetObj;
	[HideInInspector]
	public float CurrentMaxHorizontalSpeed;
	public float MaxHorizontalSpeed;
	public float MaxVerticalSpeed = -1;
	public static Vector2 BaseGravity = new Vector2(0, -200);
	public static Vector2 Gravity = BaseGravity;
	public float GroundFriction = 10.0f;
	public float AirFriction = 0.0f;
	public int MaxDoubleJumpCount = 4;
	[HideInInspector]
	public float HorizontalMoveAmount = 0;
	[HideInInspector]
	public float VerticalMoveAmount = 0;

    public bool DidDoubleJump { get { return doubleJumpCount > 0; } }
    public bool IsGrounded { get { return Feet.IsGrounded; } }

	public event Action OnLand;

	private Vector2 acc;
	private Vector2 impulse = Vector2.zero;
	
	private int doubleJumpCount = 0;
	private float knockbackTimer = 0;

	public Vector2 Velocity { get { return rigidbody2D.velocity; } }

	public EntityFeet Feet;

	public void Accelerate(Vector2 _acc) {
		acc += _acc;
	}

	public void Land() {
        doubleJumpCount = 0;
		if(OnLand != null)
			OnLand();
	}

	public void Knockback(Vector2 _vel, float _time) {
		if(IsGrounded && _vel.y < 0)
			_vel.y = 0;
		rigidbody2D.velocity = _vel;
		if (knockbackTimer < _time)
			knockbackTimer = _time;
	}

	public bool Jump(float _strength, bool _canDoubleJump, out bool _didDoubleJump) {
		_didDoubleJump = false;
		Vector2 vel = rigidbody2D.velocity;
		if (IsGrounded) {
			doubleJumpCount = 0;
			vel.y = _strength;
			Feet.Unground ();
			rigidbody2D.velocity = vel;
			return true;
		} else if(doubleJumpCount < MaxDoubleJumpCount && _canDoubleJump) {
			vel.y = _strength;
			Feet.Unground ();
			doubleJumpCount++;
			if((doubleJumpCount + 1) % 2 == 0)
				_didDoubleJump = true;
			rigidbody2D.velocity = vel;
			return true;
		}
		return false;
	}

    public void EndJump()
    {
        float ySpeed = Velocity.y;
        if(ySpeed > 0)
        {
            Impulse(new Vector2(Velocity.x, ySpeed / 4));
        }
    }

	public void Impulse(Vector2 _vel) {
		impulse = _vel;
	}

	void Start () {
		Feet = FeetObj.GetComponent<EntityFeet> ();
		CurrentMaxHorizontalSpeed = MaxHorizontalSpeed;
	}
	
	void FixedUpdate () {

		if (knockbackTimer > 0)
			knockbackTimer -= Time.fixedDeltaTime;

		if (knockbackTimer <= 0) {
			Accelerate (HorizontalMoveAmount * Vector2.right);
			Accelerate (VerticalMoveAmount * Vector2.up);
		}

		if(impulse != Vector2.zero) {
			rigidbody2D.velocity = impulse;
			impulse = Vector2.zero;
		}

		applyGravity ();
		
		if(knockbackTimer <= 0) {
			applyFriction(GroundFriction);
		}

		Vector2 vel = rigidbody2D.velocity;
		vel += acc * Time.fixedDeltaTime;
		rigidbody2D.velocity = vel;

		if (knockbackTimer <= 0) {
			if (IsGrounded)
				applyFriction (GroundFriction);
			else
				applyFriction (AirFriction);

			limitMovementSpeed ();
		}
		acc = Vector2.zero;
	}

	void applyFriction(float _amount) {
		Vector2 vel = rigidbody2D.velocity;
		vel.x -= vel.x * _amount * Time.deltaTime;
		rigidbody2D.velocity = vel;
	}
	
	void applyGravity() {
		acc += Gravity;
	}

	void limitMovementSpeed ()
	{		
		Vector2 vel = rigidbody2D.velocity;
		vel.x = Mathf.Clamp (vel.x, -CurrentMaxHorizontalSpeed, CurrentMaxHorizontalSpeed);
		if(MaxVerticalSpeed > 0)
			vel.y = Mathf.Clamp (vel.y, -MaxVerticalSpeed, MaxVerticalSpeed);
		rigidbody2D.velocity = vel;
	}
}
