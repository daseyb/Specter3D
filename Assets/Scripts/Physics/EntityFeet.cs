using UnityEngine;
using System;
using System.Collections;

public class EntityFeet : MonoBehaviour
{
	public event Action<int> HitSide;
	public Transform GroundCheck;
	public float GroundRadius = 0.2f;
	public EntityPhysics Entity;
	public bool IsGrounded { get; private set; }
	public bool IsHittingSide { get { return didHitLeft || didHitRight; } }
	public LayerMask LevelMask;
	public LayerMask OneWayMask;
	public string DefaultLayer;
	public string UpLayer;

	bool didHitLeft = false;
	bool didHitRight = false;
	int upLayerInt;
	int defaultLayerInt;
	// Use this for initialization
	void Start ()
	{
		upLayerInt = LayerMask.NameToLayer(UpLayer);
		defaultLayerInt = LayerMask.NameToLayer(DefaultLayer);
	}

	void FixedUpdate() {
		bool prevIsGrounded = IsGrounded;

		LayerMask groundMask = LevelMask;
		if (Entity.Velocity.y > 0) {
			gameObject.layer = upLayerInt;

		} else {
			gameObject.layer = defaultLayerInt;
			groundMask |= OneWayMask;
		}

		/*bool groundLeft = CheckGround (-0.3f);
		bool groundMiddle = CheckGround (0);
		bool groundRight = CheckGround (0.3f);*/
		IsGrounded = Physics2D.OverlapCircle (GroundCheck.position, GroundRadius, groundMask);

		bool hitLeft = TestDirection (-Vector2.right, groundMask);
		bool hitRight = TestDirection (Vector2.right, groundMask);

		if(HitSide != null) {
			if(hitLeft)
				HitSide(-1);
			if(hitRight)
				HitSide(1);
		}

		didHitLeft = hitLeft;
		didHitRight = hitRight;

		if (IsGrounded && IsGrounded != prevIsGrounded && Entity.Velocity.y < 0) {
			Entity.Land();
		}

	}

	bool TestDirection(Vector2 _dir, LayerMask _mask) {
		return Physics2D.Linecast (transform.position, (Vector2)transform.position + _dir * GroundRadius, _mask);
	}

	float max(params float[] _values) {
		float max = float.MinValue;
		for (int i = 0; i < _values.Length; i++)
			if (_values [i] > max)
				max = _values [i];
		return max;
	}


	public void Unground() {
		IsGrounded = false;
	}
	
}

