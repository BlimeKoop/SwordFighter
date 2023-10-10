using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public PhotonView player;
	
	public List<Transform> spawnPoints;
	
	private UIController uiController;
	
	void Awake()
	{
		uiController = transform.Find("Canvas").GetComponent<UIController>();
		
		uiController.DisableStartButton();
		uiController.DisableWinText();
	}
	
    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
	
	public override void OnConnectedToMaster()
	{
		uiController.EnableStartButton();
	}

	public void Play()
	{
		PhotonNetwork.JoinRandomRoom();
	}
	
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("Tried to join a room and failed");
		
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
	}
	
	public override void OnJoinedRoom()
	{
		Debug.Log("Joined a room");
		
		uiController.DisableStartButton();
		
		int index = PhotonNetwork.CurrentRoom.PlayerCount % 2;
		
		PhotonNetwork.Instantiate(player.gameObject.name, spawnPoints[index].position, spawnPoints[index].rotation);
	}
}
