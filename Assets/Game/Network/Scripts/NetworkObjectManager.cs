using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkObjectManager : MonoBehaviour
{
	public GameObject _MatchManagerPrefab;			private static GameObject MatchManagerPrefab;
	public GameObject _MatchObjectManagerPrefab;	private static GameObject MatchObjectManagerPrefab;
	public GameObject _MatchAudioManagerPrefab;		private static GameObject MatchAudioManagerPrefab;
	
	private void Start()
	{
		MatchManagerPrefab = _MatchManagerPrefab;
		MatchObjectManagerPrefab = _MatchObjectManagerPrefab;
		MatchAudioManagerPrefab = _MatchAudioManagerPrefab;
	}
	
	public static void SpawnMatchManagers()
	{
		PhotonNetwork.Instantiate(MatchManagerPrefab.name, new Vector3(), new Quaternion());
		PhotonNetwork.Instantiate(MatchObjectManagerPrefab.name, new Vector3(), new Quaternion());
		PhotonNetwork.Instantiate(MatchAudioManagerPrefab.name, new Vector3(), new Quaternion());
	}
}
