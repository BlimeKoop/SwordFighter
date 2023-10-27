using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using DynamicMeshCutter;

public class NetworkSceneManager : MonoBehaviourPunCallbacks
{
	private static NetworkSceneManager objectRef;
	
	private static bool loadLevelOnLeaveRoom;
	private static string loadName;

	private void Awake()
	{
		DontDestroyOnLoad(this);
		
		objectRef = this;
	}

    public static void LoadLevel(string _loadName)
    {
		string fromLevelName = SceneManager.GetActiveScene().name;
		
		loadName = _loadName;
		
		if (loadName == "Menu" && PhotonNetwork.InRoom)
		{
			loadLevelOnLeaveRoom = true;
			
			PhotonNetwork.LeaveRoom();
			
			return;
		}
		
		PhotonNetwork.LoadLevel(loadName);
		
		if (loadName == "Multiplayer" && fromLevelName != loadName)
		{
			objectRef.StopCoroutine(objectRef.SpawnMatchManagers());
			objectRef.StartCoroutine(objectRef.SpawnMatchManagers());
		}
    }
	
	private IEnumerator SpawnMatchManagers()
	{
		yield return new WaitUntil (() => PhotonNetwork.LevelLoadingProgress == 1);
		
		NetworkObjectManager.SpawnMatchManagers();
	}
	
	public override void OnLeftRoom()
	{
		if (loadLevelOnLeaveRoom)
		{
			PhotonNetwork.LoadLevel(loadName);
			loadLevelOnLeaveRoom = false;
		}
	}
}
