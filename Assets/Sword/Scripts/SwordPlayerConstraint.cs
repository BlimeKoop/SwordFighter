using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPlayerConstraint : MonoBehaviour
{
	private Rigidbody rigidbody;
	
	public Rigidbody playerRB;
	private Transform playerRBProxy;
	
	private Vector3 posStore, localPosStore;
	// private Quaternion rotStore, localRotStore;
	
	[HideInInspector]
	public Vector3 positionOffset;
	
	// [HideInInspector]
	// public Quaternion rotationOffset;
	
	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		playerRBProxy = new GameObject(gameObject.name + "Player Rigidbody Proxy").transform;
		
		SyncronizeProxy();
		RecordOffsets();
	}
	
	public void SyncronizeProxy()
	{
		playerRBProxy.position = (
			playerRB.position +
			playerRB.velocity *
			Time.fixedDeltaTime);
		
		playerRBProxy.rotation = playerRB.rotation;
		
		/* uncomfirmed if this works or not
		playerRBProxy.rotation = (
			playerRB.rotation *
			Quaternion.Euler(playerRB.angularVelocity * Time.fixedDeltaTime));
		*/
			
		playerRBProxy.localScale = playerRB.transform.lossyScale;
	}
	
	public void RecordOffsets()
	{
		localPosStore = playerRBProxy.InverseTransformPoint(rigidbody.position);
		posStore = rigidbody.position;
	}
	
	public void CalculateOffsets()
	{
		SyncronizeProxy();
		positionOffset = playerRBProxy.TransformPoint(localPosStore) - posStore;
	}
}