using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SwordCollisionController : MonoBehaviour
{
	private PlayerSwordController swordController;

	private PlayerController playerController;
	
	private BoxCollider boxCol;
	private Renderer rend;
	
	[HideInInspector] public bool cutting;
	[HideInInspector] public bool colliding
	{
		get { return collision != null; }
		set {}
	}
	
	private int layerStore;
	
	[HideInInspector] public SwordCollision collision;
	
	public void Initialize(PlayerSwordController swordController)
	{
		this.swordController = swordController;
		
		GameObject meshObj = playerController.swordModel.GetComponentInChildren<MeshFilter>().gameObject;
		boxCol = meshObj.GetComponent<BoxCollider>();
		
		if (boxCol == null)
			boxCol = meshObj.AddComponent<BoxCollider>();

		boxCol.isTrigger = false;
		boxCol.gameObject.layer = Collisions.SwordLayer;
		
		rend = playerController.swordModel.GetComponent<Renderer>();
	}

	private void OnCollisionEnter(Collision col)
	{
		if (cutting)
			return;
		
		if (colliding)
			return;
		
		if (playerController == null)
		{
			this.enabled = false;
			
			return;
		}
		
		if (col.collider.GetComponentInParent<PlayerController>() == playerController)
			return;

		if (swordController.TryShatter(col.gameObject))
			return;

		if (swordController.TryCut(col))
		{
			cutting = true;
			boxCol.gameObject.layer = Collisions.PhaseLayer;
			
			// swordController.physicsController.RevertVelocity();
			
			if (col.gameObject.name.Contains("Player"))
				GameObject.Find("UI Controller").GetComponent<UIController>().EnableWinText();
			
			StartCoroutine(StopCutting(swordController.physicsController.velocity.magnitude * Time.fixedDeltaTime));
			
			return;
		}

		if (!colliding)
		{
			colliding = true;
			
			// Debug.Log($"Colliding with {col.gameObject}");
					
			collision = SwordCollisions.SwordCollision(swordController, col);
			
			layerStore = col.collider.gameObject.layer;
			col.collider.gameObject.layer = Collisions.IgnoreSelfLayer;
			
			boxCol.gameObject.layer = Collisions.IgnoreSelfLayer;
			
			StartCoroutine(CheckStillColliding(0.05f));
		}
	}
	
	private IEnumerator StopCutting(float delay)
	{
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		
		yield return new WaitForSeconds(delay);
		
		cutting = false;
		boxCol.gameObject.layer = Collisions.SwordLayer;
	}

	private IEnumerator CheckStillColliding(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		if (StillColliding())
		{
			StartCoroutine(CheckStillColliding(delay));
			yield break;
		}
		
		StopPhasing();
		StopColliding();
	}
	
	private void StopColliding()
	{
		collision = null;
	}
	
	private void StopPhasing()
	{
		collision.collider.gameObject.layer = layerStore;
		boxCol.gameObject.layer = Collisions.SwordLayer;
	}
	
	private bool StillColliding()
	{
		Collider[] overlapBox = Physics.OverlapBox(
			ColliderCheckOrigin(),
			ColliderCheckExtents(),
			swordController.rollController.rotation,
			(1 << Collisions.IgnoreSelfLayer));
		
		if (overlapBox.Length > 1)
			return true;
		
		return false;
	}
	
	private Vector3 ColliderCheckOrigin()
	{
		return (
			boxCol.transform.TransformPoint(boxCol.center) +
			transform.forward *
			swordController.length *
			swordController.grabPointRatio);
	}
	
	private Vector3 ColliderCheckExtents()
	{
		return Vector3.Scale(
			Objects.BoxColliderExtents(boxCol, swordController.rollController),
			new Vector3(2.0f, 1.4f, 1.0f - swordController.grabPointRatio));
	}
	
	public BoxCollider Collider() { return boxCol; }
	
	public void SetPlayerController(PlayerController playerController) { this.playerController = playerController; }
}
