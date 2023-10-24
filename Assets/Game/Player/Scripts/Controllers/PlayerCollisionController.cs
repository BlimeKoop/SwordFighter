using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
	[HideInInspector] public PlayerController playerController;
	private PlayerAnimationController animationController;
	private PlayerPhysicsController physicsController;
	
	[HideInInspector] public Dictionary<string, Collider> colliders = new Dictionary<string, Collider>();
	[HideInInspector] public Dictionary<string, Collision> collisions = new Dictionary<string, Collision>()
	{
		{"w" , null},
	};
	
	[HideInInspector] public Dictionary<string, bool> collisionFlags = new Dictionary<string, bool>()
	{
		{"w" , false}
	};
	
	public RaycastHit groundHit;
	public float groundDistance;
	public bool onGround;
	public bool onWall { get { return collisionFlags["w"]; } }
	
	private bool initialized;
	
	public void Initialize(PlayerController playerController)
	{
		this.playerController = playerController;
		animationController = playerController.animationController;
		physicsController = playerController.physicsController;
		
		colliders["n"] = animationController.bones["n"].GetComponent<Collider>();
		colliders["h"] = animationController.bones["h"].GetComponent<Collider>();
		colliders["lf"] = animationController.bones["lf"].GetComponentInChildren<Collider>();
		colliders["rf"] = animationController.bones["rf"].GetComponentInChildren<Collider>();
	
		initialized = true;
	}
	
	public void DoUpdate()
	{
		bool onGroundStore = onGround;
		
		onGround = PlayerDetection.GroundHit(playerController, out groundHit, out groundDistance);
		
		if (onGroundStore && !onGround && !playerController.lockJump)
		{
			playerController.StopCoroutine(playerController.JumpGracePeriod());
			playerController.StartCoroutine(playerController.JumpGracePeriod());
		}
		else if (!onGroundStore && onGround && playerController.jumpInputGrace)
		{
			groundHit = new RaycastHit();
			
			playerController.Jump();
		}
	}
	
	private void OnCollisionEnter(Collision col)
	{
		if(playerController.dead || !playerController.photonView.IsMine || !initialized)
			return;
		
		if (col.transform == playerController.sword)
			return;
		
		if (Mathf.Abs(col.GetContact(0).normal.y) < 0.6f)
		{
			collisionFlags["w"] = true;
			collisions["w"] = col;
		}
	}
	
	private void OnCollisionExit(Collision col)
	{
		if(playerController.dead || !playerController.photonView.IsMine || !initialized)
			return;
		
		if (collisionFlags["w"] && col.collider == collisions["w"].collider)
		{
			collisionFlags["w"] = false;
		}
	}
	
	public float VerticalGroundOffset()
	{
		return groundHit.point.y - physicsController.rigidbody.position.y;
	}
}
