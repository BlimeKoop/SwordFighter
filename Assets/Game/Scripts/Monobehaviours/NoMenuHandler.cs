using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NoMenuHandler : MonoBehaviourPunCallbacks
{
	private void Start()
	{
        if (!PhotonNetwork.IsConnected)
        {
			// PlayTheme();
			// uiController.EnableBackground();
			
            PhotonNetwork.ConnectUsingSettings();
        }
	}
	
    public override void OnConnectedToMaster()
	{
		UIController.EnableStartButton();
	}
	
	public void OnStartButtonClicked()
	{
        if (!PhotonNetwork.IsConnectedAndReady)
        {
			UIController.DisableStartButton();
            PhotonNetwork.ConnectUsingSettings();

            return;
        }
		
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
		PhotonNetwork.JoinRandomRoom();
	}
	
	public override void OnJoinedRoom()
	{
		Debug.Log("Joined a room");
		
        MatchManager.SetUpRoom();
	}
}
