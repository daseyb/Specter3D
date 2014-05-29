using UnityEngine;
using System.Collections;

public enum PlayerTriggerType
{
    Stab,
    Jump
}

public class PlayerAnimationController : MonoBehaviour
{
    public Rigidbody2D MovementRigidbody;
    public EntityPhysics EntityPhysics;

    private Animator anim;

    public void SetFacing(int _dir)
    {
        var scale = transform.localScale;
        scale.x = _dir;
        transform.localScale = scale;
    }

    public void ActivateTrigger(PlayerTriggerType type)
    {
        anim.SetTrigger(type.ToString());
    }

	// Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    anim.SetFloat("speed", MovementRigidbody.velocity.magnitude);
        anim.SetBool("onGround", EntityPhysics.IsGrounded);
	}
}
