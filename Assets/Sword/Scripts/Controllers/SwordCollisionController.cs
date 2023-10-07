using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionController : MonoBehaviour
{
	private PlayerSwordController swordController;
	private SwordPhysicsController physicsController;

	private PlayerController playerController;
	
	private BoxCollider boxCol;
	
	[HideInInspector] public bool cutting, phasing;
	[HideInInspector] public Collision collision;
	
	private void Start()
	{
		swordController = GetComponent<PlayerSwordController>();
		physicsController = GetComponent<SwordPhysicsController>();
		
		GameObject meshObj = GetComponentInChildren<MeshFilter>().gameObject;
		boxCol = meshObj.GetComponent<BoxCollider>();
		
		if (boxCol == null)
			boxCol = meshObj.AddComponent<BoxCollider>();

		boxCol.isTrigger = false;
	}
	
	private void Update()
	{
		if (collision != null && !StillColliding())
			StopColliding();
		
		if (cutting && !StillColliding())
			StopCutting();
	}
	
	private bool StillColliding()
	{
		Vector3 boxColExtents = Objects.BoxColliderExtents(gameObject);
		
		return Physics.CheckBox(
			boxCol.bounds.center, boxColExtents, physicsController.RigidbodyRotation(), ~(1 << 6));
	}

	private void OnCollisionEnter(Collision col)
	{
		// Debug.Log("Sword hit " + col.gameObject.name);
		
		if (swordController.TryShatter(col.gameObject))
		{
			return;
		}
		
		if (swordController.TryCut(col))
		{
			boxCol.isTrigger = true;
			cutting = true;
			
			return;
		}

		if (col.contacts[0].otherCollider.bounds.size.magnitude < 2f)
			return;
		
		// Debug.Log("Sword collision with " + col.gameObject.name);

		// boxCol.isTrigger = true;
		physicsController.FreezeRigidbodyUntilFixedUpdate();
		physicsController.ZeroVelocity();
		
		collision = col;
	}
	
	private void StopColliding()
	{
		boxCol.isTrigger = false;
		collision = null;
	}
	
	private void StopCutting()
	{
		boxCol.isTrigger = false;
		cutting = false;
	}
	
	public BoxCollider GetCollider() { return boxCol; }
	public bool Colliding() { return collision != null && collision.contacts.Length > 0; }
	
	public void SetPlayerController(PlayerController playerController) { this.playerController = playerController; }
}
