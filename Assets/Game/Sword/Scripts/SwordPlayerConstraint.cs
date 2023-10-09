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
		
		InitializeProxy();
		RecordOffsets();
	}
	
	private void InitializeProxy()
	{
		playerRBProxy.position = playerController.sword.position;
		playerRBProxy.rotation = playerController.sword.rotation;			
		playerRBProxy.localScale = playerController.sword.lossyScale;
	}
	
	public void SyncronizeProxy()
	{
		playerRBProxy.position = (
			playerController.physicsController.rigidbody.position +
			playerController.physicsController.rigidbody.velocity *
			Time.fixedDeltaTime);
		
		playerRBProxy.rotation = playerController.physicsController.rigidbody.rotation;
		
		/* uncomfirmed if this works or not
		playerRBProxy.rotation = (
			playerController.physicsController.rigidbody.rotation *
			Quaternion.Euler(playerRB.angularVelocity * Time.fixedDeltaTime));
		*/
			
		playerRBProxy.localScale = playerController.physicsController.rigidbody.transform.lossyScale;
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