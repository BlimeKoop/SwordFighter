using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionController : MonoBehaviour
{
	private PlayerSwordController swordController;
	private SwordPhysicsController physicsController;

	private PlayerController playerController;
	
	private BoxCollider boxCol;
	
	private bool colliding, cutting, phasing;
	
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
		if (colliding && !StillColliding())
			StopColliding();
		
		if (cutting && !StillColliding())
			StopCutting();
	}
	
	private bool StillColliding()
	{
		Vector3 boxColExtents = Objects.BoxColliderExtents(gameObject);
		Vector3 boxTip = boxCol.bounds.center + transform.forward * (boxColExtents.z + 0.1f);
		
		Vector3 boxOrigin = Vector3.Lerp(transform.position, boxTip, 0.5f) + transform.forward * 0.2f;
		Vector3 boxExtents = boxColExtents;
		boxExtents.z = Vector3.Distance(boxOrigin, boxTip);
		
		return Physics.CheckBox(boxOrigin, boxExtents, transform.rotation, ~(1 << 6));
	}

	private void OnCollisionEnter(Collision col)
	{
		// Debug.Log("Sword hit " + col.gameObject.name);
		
		if (swordController.TryShatter(col.gameObject))
		{
			physicsController.RevertVelocity();
			
			return;
		}
		
		if (swordController.TryCut(col.gameObject))
		{
			boxCol.isTrigger = true;
			cutting = true;
			
			physicsController.RevertVelocity();
			
			return;
		}

		if (col.contacts[0].otherCollider.bounds.size.magnitude < 2f)
			return;

		boxCol.isTrigger = true;
		
		colliding = true;
		
		physicsController.FreezeRigidbodyUntilFixedUpdate();
		physicsController.ZeroVelocity();
		
		playerController.Collide(col);
		swordController.Collide(col);
	}
	
	private void StopColliding()
	{
		boxCol.isTrigger = false;
		colliding = false;
		
		playerController.StopColliding();
		swordController.StopColliding();	
	}
	
	private void StopCutting()
	{
		boxCol.isTrigger = false;
		cutting = false;
	}
	
	public BoxCollider GetCollider() { return boxCol; }
	
	public void SetPlayerController(PlayerController playerController) { this.playerController = playerController; }
}
