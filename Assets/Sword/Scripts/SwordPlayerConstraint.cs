using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlayerConstraint : MonoBehaviour
{
	public PlayerController playerController;
	public PlayerSwordController swordController;
	
	private Transform playerRBProxy;
	
	private Vector3 posStore, localPosStore;
	// private Quaternion rotStore, localRotStore;
	
	[HideInInspector]
	public Vector3 positionOffset;
	
	// [HideInInspector]
	// public Quaternion rotationOffset;
	
	public void Initialize(PlayerSwordController swordController)
	{
		this.swordController = swordController;
		playerController = swordController.playerController;
		
		playerRBProxy = new GameObject(gameObject.name + "Player Rigidbody Proxy").transform;
		
		SyncronizeProxy();
		RecordOffsets();
	}
	
	public void SyncronizeProxy()
	{
		playerRBProxy.position = (
			playerController.physicsController.rigidbodySync.position +
			playerController.physicsController.rigidbodySync.velocity *
			Time.fixedDeltaTime);
		
		playerRBProxy.rotation = playerController.physicsController.rigidbodySync.rotation;
		
		/* uncomfirmed if this works or not
		playerRBProxy.rotation = (
			playerController.physicsController.rigidbodySync.rotation *
			Quaternion.Euler(playerRB.angularVelocity * Time.fixedDeltaTime));
		*/
			
		playerRBProxy.localScale = playerController.physicsController.rigidbodySync.transform.lossyScale;
	}
	
	public void RecordOffsets()
	{
		localPosStore = playerRBProxy.InverseTransformPoint(swordController.physicsController.RigidbodyPosition());
		posStore = swordController.physicsController.RigidbodyPosition();
	}
	
	public void CalculateOffsets()
	{
		SyncronizeProxy();
		positionOffset = playerRBProxy.TransformPoint(localPosStore) - posStore;
	}
}