using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlayerConstraint : MonoBehaviour
{
	public PlayerController playerController;
	public PlayerSwordController swordController;
	
	private Rigidbody swordRB;
	private Rigidbody playerRB;
	private Transform playerRBProxy;
	
	private Transform sword;
	private Transform player;
	
	private Vector3 posStore, localPosStore;
	private Quaternion rotStore; private Vector3 localForwardStore;
	
	[HideInInspector]
	public Vector3 positionOffset;
	[HideInInspector]
	public Quaternion rotationOffset;
	
	public void Initialize(PlayerSwordController swordController)
	{
		this.swordController = swordController;
		playerController = swordController.playerController;
		
		swordRB = swordController.physicsController.rigidbody;
		playerRB = playerController.GetComponentInChildren<Rigidbody>();
		playerRBProxy = new GameObject(gameObject.name + "Player Rigidbody Proxy").transform;
		
		sword = swordRB.transform;
		player = playerRB.transform;
		
		InitializeProxy();
		RecordOffsets();
	}
	
	private void InitializeProxy()
	{
		playerRBProxy.position = player.position;
		playerRBProxy.rotation = player.rotation;			
		playerRBProxy.localScale = player.lossyScale;
	}
	
	public void SyncronizeProxy()
	{
		playerRBProxy.position = playerRB.position + playerRB.velocity * Time.fixedDeltaTime;
		playerRBProxy.rotation = playerRB.rotation;
		
		/* uncomfirmed if this works or not
		playerRBProxy.rotation = playerRB.rotation * Quaternion.Euler(playerRB.angularVelocity * Time.fixedDeltaTime));
		*/
			
		playerRBProxy.localScale = player.lossyScale;
	}
	
	public void RecordOffsets()
	{
		posStore = swordRB.position;
		localPosStore = playerRBProxy.InverseTransformPoint(posStore);
		
		rotStore = swordRB.rotation;
		localForwardStore = playerRBProxy.InverseTransformDirection(sword.forward);
	}
	
	public void CalculateOffsets()
	{
		SyncronizeProxy();
		
		positionOffset = playerRBProxy.TransformPoint(localPosStore) - posStore;
		rotationOffset = Quaternion.Inverse(rotStore) * Quaternion.LookRotation(playerRBProxy.TransformDirection(localForwardStore), Vector3.up);
	}
}