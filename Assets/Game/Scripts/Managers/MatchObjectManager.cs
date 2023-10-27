using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DynamicMeshCutter;

public class MatchObjectManager : MonoBehaviour
{
	private static PhotonView photonView;
	private static SwordCutterBehaviour cutterBehaviour;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		
		photonView = GetComponent<PhotonView>();
		cutterBehaviour = GetComponent<SwordCutterBehaviour>();
	}

	public static void CutObject(GameObject obj, Vector3 cutAxis, Vector3 point)
	{
		var rend = Objects.GetComponentInFamily<Renderer>(obj);
		
		if (rend != null)
			obj = rend.gameObject;
			
		GameObject[] rigidbodies = SpawnCutRigidbodies(obj);
		
		photonView.RPC(
			"CutObject",
			RpcTarget.AllBufferedViaServer,
			obj.name,
			new string[] { rigidbodies[0].name, rigidbodies[1].name },
			obj.transform.InverseTransformDirection(cutAxis),
			obj.transform.InverseTransformPoint(point));
	}
	
	[PunRPC]
	private void CutObject(string objectName, string[] rigidbodyNames, Vector3 localAxis, Vector3 localPoint)
	{
		cutterBehaviour.CutObject(objectName, rigidbodyNames, localAxis, localPoint);
	}
	
	private static GameObject[] SpawnCutRigidbodies(GameObject obj)
	{
		string nameBase = obj.name;
		
		if (obj.GetComponentInChildren<MeshFilter>() != null)
			nameBase = obj.GetComponentInChildren<MeshFilter>().gameObject.name;
		else if (obj.GetComponentInChildren<SkinnedMeshRenderer>() != null)
			nameBase = obj.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.name;

		nameBase = Objects.StripName(nameBase);

		GameObject[] rigidbodiesR = new GameObject[]
		{
			SpawnRigidbody(
				$"{nameBase} (1/2)",
				obj.transform.position, obj.transform.rotation, obj.tag),
				
			SpawnRigidbody(
				$"{nameBase} (2/2)",
				obj.transform.position, obj.transform.rotation, obj.tag)
		};
		
		rigidbodiesR[0].gameObject.layer = obj.layer;
		rigidbodiesR[1].gameObject.layer = obj.layer;
		
		return rigidbodiesR;
	}
	
	public static GameObject SpawnRigidbody(string name, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(), string tag = "")
	{
		GameObject instance = PhotonNetwork.Instantiate("Rigidbody", position, rotation, 0, new object[] { name });
		
		if (tag == "")
			return instance;
		
		instance.tag = tag;
		return instance;
	}
	
	[PunRPC]
	private void DestroyObject(string objectName)
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
}
