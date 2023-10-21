using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DynamicMeshCutter;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public static PhotonView photonView;
	
	[HideInInspector] public SwordCutterBehaviour cutterBehaviour;
	
	public AudioSource themeSource;
	public AudioSource loopSource;
	
	public int _deathPlaneHeight = -50;
	
	public static int deathPlaneHeight;
	public static int spawnedPlayerCount;
	
	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		cutterBehaviour = GetComponent<SwordCutterBehaviour>();
	}
	
	private void Start()
	{
		deathPlaneHeight = _deathPlaneHeight;
	}

	[PunRPC]
	public void LoadLevel(int index)
	{
		cutterBehaviour.StopCoroutines();
		
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
	
	public static GameObject SpawnRigidbody(string name, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(), string tag = "")
	{
		GameObject instance = PhotonNetwork.Instantiate("Rigidbody", position, rotation, 0, new object[] { name });
		
		if (tag == "")
			return instance;
		
		instance.tag = tag;
		return instance;
	}
	
	public static void CutObject(GameObject obj, Vector3 cutAxis, Vector3 point)
	{
		GameObject[] rigidbodies = SpawnCutRigidbodies(obj);
		
		photonView.RPC(
			"CutObject",
			RpcTarget.AllBufferedViaServer,
			obj.name,
			new string[] { rigidbodies[0].name, rigidbodies[1].name },
			cutAxis,
			point);
	}
	
	[PunRPC]
	private void CutObject(string objectName, string[] rigidbodyNames, Vector3 cutAxis, Vector3 point)
	{
		cutterBehaviour.CutObject(objectName, rigidbodyNames, cutAxis, point);
	}
	
	private static GameObject[] SpawnCutRigidbodies(GameObject obj)
	{
		string nameBase = obj.name;
		
		if (obj.GetComponentInChildren<MeshFilter>() != null)
			nameBase = obj.GetComponentInChildren<MeshFilter>().gameObject.name;
		else if (obj.GetComponentInChildren<SkinnedMeshRenderer>() != null)
			nameBase = obj.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.name;

		return new GameObject[] {
			RoomManager.SpawnRigidbody(
				$"{nameBase} (1/2)",
				obj.transform.position, obj.transform.rotation, obj.tag),
				
			RoomManager.SpawnRigidbody(
				$"{nameBase} (2/2)",
				obj.transform.position, obj.transform.rotation, obj.tag)};
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
