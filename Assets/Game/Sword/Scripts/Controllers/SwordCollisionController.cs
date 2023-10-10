using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwordCollisionController : MonoBehaviour
{
	private PlayerSwordController swordController;

	private PlayerController playerController;
	
	private BoxCollider boxCol;
	
	[HideInInspector] public bool cutting, phasing;
	[HideInInspector] public Collision collision;
	
	public void Initialize(PlayerSwordController swordController)
	{
		this.swordController = swordController;
		
		GameObject meshObj = playerController.swordModel.GetComponentInChildren<MeshFilter>().gameObject;
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
			boxCol.bounds.center,
			boxColExtents * 1.3f,
			swordController.physicsController.RigidbodyRotation(),
			~(1 << 6));
	}

	private void OnCollisionEnter(Collision col)
	{
		if (playerController == null)
		{
			this.enabled = false;
			
			return;
		}
		
		if (col.gameObject == playerController.gameObject)
			return;

		if (col.GetContact(0).thisCollider != boxCol)
			return;
		
		if (swordController.TryShatter(col.gameObject))
			return;

		if (swordController.TryCut(col))
		{
			StopColliding();
			
			boxCol.isTrigger = true;
			cutting = true;
			
			if (col.gameObject.name.Contains("Player"))
				GameObject.Find("UIController").GetComponent<UIController>().EnableWinText();
			
			return;
		}
		
		// Debug.Log("Sword collision with " + col.gameObject.name);

		// boxCol.isTrigger = true;
		// physicsController.FreezeRigidbodyUntilFixedUpdate();
		swordController.physicsController.Zero();
		
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
