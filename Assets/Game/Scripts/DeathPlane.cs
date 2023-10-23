using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeathPlane : MonoBehaviour
{
	public int height;
	
	private BoxCollider boxCol;
	
	bool initialized;
	
	private void Start()
	{
		if (!initialized)
			Initialize(height);
	}
	
	private void OnValidate()
	{
		Initialize(height);
	}
	
	public void Initialize(int height)
	{
		this.height = height;

		boxCol = GetComponent<BoxCollider>();
		
		if (boxCol == null)
			boxCol = gameObject.AddComponent<BoxCollider>();

		boxCol.isTrigger = true;
		
		transform.localScale = new Vector3(10000f, 100f, 10000f);
		transform.position = Vector3.up * (height - 100f / 2);
		
		initialized = true;
	}
	
    private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer == Collisions.PlayerLayer || col.gameObject.layer == Collisions.SwordLayer)
			return;
		
		if (col.GetComponentInParent<PhotonView>() != null)
		{
			if (PhotonNetwork.IsMasterClient)
				PhotonNetwork.Destroy(col.GetComponentInParent<PhotonView>().gameObject);
			else
				return;
		}
		else
			Destroy(col.gameObject);
	}
}
