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
	
	private List<GameObject> collidingObjects = new List<GameObject>();
	private List<GameObject> cuttingObjects = new List<GameObject>();
	
	[HideInInspector] public bool cutting;
	[HideInInspector] public bool colliding;
	
	private int layerStore;
	
	[HideInInspector] public SwordCollision collision;
	
	public void Initialize(PlayerSwordController swordController)
	{
		this.swordController = swordController;
		
		GameObject meshObj = playerController.swordModel.GetComponentInChildren<MeshFilter>().gameObject;
		boxCol = meshObj.GetComponentInChildren<BoxCollider>();
		
		if (boxCol == null)
		{
			foreach (Collider c in meshObj.GetComponentsInChildren<Collider>())
				Destroy(c);
				
			boxCol = meshObj.AddComponent<BoxCollider>();
		}

		boxCol.isTrigger = false;
		boxCol.gameObject.layer = Collisions.SwordLayer;
		
		rend = playerController.swordModel.GetComponent<Renderer>();
	}

	private void OnCollisionEnter(Collision col)
	{
		if (cutting || colliding)
			return;
		
		if (playerController == null || !playerController.photonView.IsMine)
		{
			this.enabled = false;
			
			return;
		}
		
		if (col.gameObject.layer == Collisions.SwordLayer)
		{
			SwordAudioManager.PlayImpact(swordController.physicsController.rigidbody.velocity.magnitude / 7f);
			return;
		}

		if (col.gameObject.layer == Collisions.PlayerLayer)
		{
			HandlePlayerCollision(col.collider);
			return;
		}

		if (swordController.TryShatter(col.gameObject))
			return;

		if (TryCut(col.collider, col.GetContact(0).point, col.rigidbody))
		{			
			StartCut(col.gameObject);

			return;
		}
		
		if (!playerController.dead && !colliding)
		{
			colliding = true;
			
			collidingObjects.Add(col.gameObject);
			
			// Debug.Log($"Colliding with {col.gameObject}");
					
			collision = SwordCollisions.SwordCollision(swordController, col);
			
			layerStore = col.collider.gameObject.layer;
			
			col.collider.gameObject.layer = Collisions.IgnoreSelfLayer;
			boxCol.gameObject.layer = Collisions.IgnoreSelfLayer;
			
			StopCoroutine(StopColliding());
			StartCoroutine(StopColliding());
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (cutting || colliding)
			return;
		
		if (playerController == null || !playerController.photonView.IsMine)
		{
			this.enabled = false;
			
			return;
		}		
		
		if (col.gameObject.layer == Collisions.PlayerLayer)
			HandlePlayerCollision(col);
	}
	
	private void HandlePlayerCollision(Collider col)
	{
		if (col.GetComponentInParent<PlayerController>() == playerController)
			return;

		if (TryCut(col, col.ClosestPoint(swordController.MiddlePoint()), col.GetComponentInParent<Rigidbody>()))
		{
			StartCut(col.gameObject);
			
			if (PlayerData.playerMatchData[col.gameObject.GetComponentInParent<PhotonView>().Controller.ActorNumber].lives > 1)
				SwordAudioManager.PlayFleshSlice(swordController.physicsController.rigidbody.velocity.magnitude / 7f);
			else
				SwordAudioManager.PlayNiceSlice(0.8f);
			
			return;
		}
	}
	
	private bool TryCut(Collider col, Vector3 point, Rigidbody rigidbody)
	{
		if (cuttingObjects.Contains(col.gameObject))
			return false;

		return swordController.TryCut(col.gameObject, point, rigidbody);
	}

	private void StartCut(GameObject obj)
	{
		cutting = true;
		boxCol.gameObject.layer = Collisions.PhaseLayer;
		
		// swordController.physicsController.RevertVelocity();
		
		if (obj.layer == Collisions.PlayerLayer)
			UIController.EnableWinText();
		
		// This may not be necessary
		StartCoroutine(MonitorCuttingObject(obj));
		
		StopCoroutine(StopCutting());
		StartCoroutine(StopCutting());
	}
	
	public IEnumerator MonitorCuttingObject(GameObject obj)
	{
		cuttingObjects.Add(obj);
		
		float overloadTimer = 0;
		
		// Attempt to Wait until it's been destroyed
		while(obj != null && overloadTimer < 3f)
		{
			overloadTimer += Time.deltaTime;
			yield return null;
		}
		
		cuttingObjects.Remove(obj);
	}

	private IEnumerator StopColliding(float delay = 0.02f)
	{
		yield return new WaitForSeconds(delay);
		
		if (StillColliding())
		{
			StartCoroutine(StopColliding(delay));
			yield break;
		}
		
		StopPhasing();
		
		collision = null;
		colliding = false;
	}
	
	private IEnumerator StopCutting(float delay = 0.05f)
	{
		yield return new WaitForSeconds(delay);
		
		if (StillCutting())
		{
			StartCoroutine(StopCutting(delay));
			yield break;
		}
		
		boxCol.gameObject.layer = Collisions.SwordLayer;
		cutting = false;
	}
	
	private void StopPhasing()
	{
		if (collision != null && collision.collider != null)
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
		
		foreach(Collider c in overlapBox)
			if (collidingObjects.Contains(c.gameObject))
				return true;
		
		return false;
	}
	
	private bool StillCutting()
	{
		Collider[] overlapBox = Physics.OverlapBox(
			ColliderCheckOrigin(),
			ColliderCheckExtents(),
			swordController.rollController.rotation,
			~(1 << Collisions.PhaseLayer));
		
		foreach(Collider c in overlapBox)
			if (cuttingObjects.Contains(c.gameObject))
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
