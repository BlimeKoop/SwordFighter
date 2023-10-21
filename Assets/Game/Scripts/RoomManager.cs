using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public static PhotonView photonView;
	
	public AudioSource themeSource;
	public AudioSource loopSource;
	
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
	
	public static void InstantiateRigidbody(string name, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion())
	{
		PhotonNetwork.Instantiate("Rigidbody", target.transform.position, target.transform.rotation, 0, new object[] { name }).transform;
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
		
		Debug.Log($"Destroying {objectName}");
		Destroy(obj);
	}
	
	[PunRPC]
	public void NetworkDestroyObject(string objectName)
	{
		GameObject obj = GameObject.Find(objectName);
		
		if (obj == null)
		{
			Debug.Log($"Can't find {objectName}");
			return;
		}
		
		if (obj.GetComponent<PhotonView>() == null)
		{
			Debug.Log($"Can't network destroy {objectName} without a photonView");
			return;
		}
		
		if (!obj.GetComponent<PhotonView>().AmOwner)
			return;
		
		Debug.Log($"Network destroying {objectName}");
		PhotonNetwork.Destroy(obj);
	}
	
	public void DestroyAudioListener()
	{
		if (GetComponent<AudioListener>() != null)
			Destroy(GetComponent<AudioListener>());
	}
	
	public void PlayTheme()
	{
		themeSource.Play();
	}
	
	public void PlayMusic()
	{
		loopSource.Play();
	}

	public void StopMusic()
	{
		loopSource.Stop();
	}
}
