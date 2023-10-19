using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public static PhotonView photonView;
	
	public int _deathPlaneHeight = -50;
	
	public static int deathPlaneHeight;
	public static int spawnedPlayerCount;
    public static int sliceCount;
	
	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
	}
	
	private void Start()
	{
		deathPlaneHeight = _deathPlaneHeight;
	}
	
	[PunRPC]
	public void LoadLevel(int index)
	{
		PhotonNetwork.LocalPlayer.CustomProperties["spawned"] = false;
		PhotonNetwork.LoadLevel(index);
	}

    [PunRPC]
	public void IncreasePlayerCount(int increment)
	{
		spawnedPlayerCount += increment;
	}

    [PunRPC]
    public void DecreasePlayerCount(int decrement)
    {
        spawnedPlayerCount -= decrement;
    }
	
	[PunRPC]
	public void DestroyObject(string objectName)
	{
		GameObject obj = GameObject.Find(objectName);
		
		if (obj == null)
		{
			Debug.Log($"Can't find {objectName}");
			return;
		}
		
		if (!obj.GetComponent<PhotonView>().AmOwner)
			return;
		
		PhotonNetwork.Destroy(obj);
	}
}
