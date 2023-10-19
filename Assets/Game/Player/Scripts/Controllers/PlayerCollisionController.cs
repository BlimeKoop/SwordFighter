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
		{"g" , null},
		{"w" , null},
		{"t" , null},
	};
	
	[HideInInspector] public Dictionary<string, bool> collisionFlags = new Dictionary<string, bool>()
	{
		{"t" , false}
	};
	
	public RaycastHit groundHit;
	public bool onGround;
	public bool onWall { get { return collisionFlags["t"]; } }
	
	private bool initialized;
	
	public void Initialize(PlayerController playerController)
	{
		this.playerController = playerController;
		animationController = playerController.animationController;
		physicsController = playerController.physicsController;
		
		colliders["t"] = animationController.bones["s"].GetComponent<Collider>();
		colliders["lf"] = animationController.bones["lf"].GetComponentInChildren<Collider>();
		colliders["rf"] = animationController.bones["rf"].GetComponentInChildren<Collider>();
	
		initialized = true;
	}
	
	public void DoUpdate()
	{
		groundHit = PlayerDetection.GroundHit(playerController);
		
		onGround = groundHit.transform != null;
	}
		
	private void OnCollisionEnter(Collision col)
	{
		if(playerController.dead || !playerController.photonView.IsMine || !initialized)
			return;
		
		if (col.transform == playerController.sword)
			return;
		
		if (col.GetContact(0).thisCollider == (colliders["t"]))
		{
			HandleTorsoeCollision(col);
		}
		else if(col.GetContact(0).thisCollider == colliders["lf"])
		{
			HandleLeftFootCollision(col);
		}
		else if(col.GetContact(0).thisCollider == colliders["rf"])
		{
			HandleRightFootCollision(col);
		}
	}
	
	private void OnCollisionExit(Collision col)
	{
		if(playerController.dead || !playerController.photonView.IsMine || !initialized)
			return;
		
		if (collisionFlags["t"] && col.collider == collisions["t"].collider)
		{
			collisionFlags["t"] = false;
		}
	}
	
	private void HandleTorsoeCollision(Collision col)
	{
		collisions["t"] = col;
		collisionFlags["t"] = true;
	}
	
	private void HandleLeftFootCollision(Collision col)
	{
		if (Mathf.Abs(col.GetContact(0).normal.y) < 0.7f)
		{
			collisions["w"] = col;
			collisionFlags["t"] = true;
		}
		// else
			// collisions["g"] = col;
		
		// collisions["lf"] = col;
	}
	
	private void HandleRightFootCollision(Collision col)
	{
		if (Mathf.Abs(col.GetContact(0).normal.y) < 0.7f)
		{
			collisions["w"] = col;
			collisionFlags["t"] = true;
		}
		// else
			// collisions["g"] = col;
		
		// collisions["rf"] = col;
	}
	
	public float VerticalGroundOffset()
	{
		return groundHit.point.y - physicsController.rigidbody.position.y;
	}
}
