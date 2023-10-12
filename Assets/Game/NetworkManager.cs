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

    void Start()
    {
		uiController = GameObject.Find("UIController").GetComponent<UIController>();
		
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
		if (!PhotonNetwork.IsConnected)
		{
			Connect();

            return;
		}

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

		int index = (PhotonNetwork.CurrentRoom.PlayerCount - 1) % 2;

        PhotonNetwork.Instantiate(player.gameObject.name, spawnPoints[index].position, spawnPoints[index].rotation);
	}
}
