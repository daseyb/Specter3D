using UnityEngine;
using System;
using System.Collections;

public class EntityFeet : MonoBehaviour
{
	public event Action<int> HitSide;
	public Transform GroundCheck;
	public float GroundRadius = 0.2f;
	public EntityPhysics Entity;
    public bool IsGrounded { get { return isGrounded && timeSinceUnground > 0.05f; } }
	public bool IsHittingSide { get { return didHitLeft || didHitRight; } }
	public LayerMask LevelMask;
	public LayerMask OneWayMask;
	public string DefaultLayer;
	public string UpLayer;

    private bool isGrounded = false;
	bool didHitLeft = false;
	bool didHitRight = false;
	int upLayerInt;
	int defaultLayerInt;

    float timeSinceUnground = 1.0f;

	// Use this for initialization
	void Start ()
	{
		upLayerInt = LayerMask.NameToLayer(UpLayer);
		defaultLayerInt = LayerMask.NameToLayer(DefaultLayer);
	}

    void Update()
    {
        timeSinceUnground += Time.deltaTime;
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

        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundRadius, groundMask);

		var hitLeft = TestDirection (-transform.right, groundMask);
		var hitRight = TestDirection (transform.right, groundMask);

		if(HitSide != null) {
			if(hitLeft)
				HitSide(-1);
			if(hitRight)
				HitSide(1);
		}

		didHitLeft = hitLeft;
		didHitRight = hitRight;

        if (isGrounded)
        {
            var hitGround = TestDirection(-transform.up, groundMask);
            Entity.transform.up = hitGround.normal;
        }

        if (IsGrounded && isGrounded != prevIsGrounded && Entity.Velocity.y < 0)
        {
			Entity.Land();
		}

	}

	RaycastHit2D TestDirection(Vector2 _dir, LayerMask _mask) {
		return Physics2D.Linecast (transform.position, (Vector2)transform.position + _dir * GroundRadius, _mask);
	}


	public void Unground() {
		isGrounded = false;
        timeSinceUnground = 0;
	}
	
}

