using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public PhotonView player;
	public RoomManager roomManager;
	
	public Transform spawnPoints;

	private UIController uiController;
	private PhotonView roomManagerView;

    private bool loadLevel, switchingMasterClient;
    private int loadIndex;

    private Hashtable CustomProperties = new Hashtable();

    private void Start()
    {
        uiController = GameObject.Find("UI Controller").GetComponent<UIController>();
        roomManagerView = GameObject.Find("Room Manager").GetComponent<PhotonView>();

        if (!PhotonNetwork.IsConnected)
        {
			/*
			roomManager.PlayTheme();
			uiController.EnableBackground();
			
            ConfigureLocalPlayer();
            Connect();
			*/
        }

        if (PhotonNetwork.InRoom)
		{
			roomManager.DestroyAudioListener();
			roomManager.PlayMusic();
			
            SpawnThings();
		}
	}

    private void ConfigureLocalPlayer()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(CustomProperties);

        CustomProperties["spawned"] = false;
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
	
    private void SpawnThings()
    {
        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.childCount;

        PhotonNetwork.Instantiate(player.gameObject.name, spawnPoints.GetChild(index).position, spawnPoints.GetChild(index).rotation);
        PhotonNetwork.LocalPlayer.CustomProperties["spawned"] = true;

        roomManagerView.RPC("IncreasePlayerCount", RpcTarget.AllBuffered, 1);
    }

	public void LoadLevel(int index)
	{
        if (switchingMasterClient && !FinishSwitchingMasterClient())
            return;

        loadLevel = true;
        loadIndex = index;

        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);

            switchingMasterClient = true;

            return;
        }

        LoadLevel();
    }

    private bool FinishSwitchingMasterClient()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            switchingMasterClient = false; 

            return true;
        }

        return false;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        switchingMasterClient = false;

        if (loadLevel)
            LoadLevel(loadIndex);
    }

    private void LoadLevel()
    {
        PhotonNetwork.DestroyAll();

        roomManagerView.RPC("LoadLevel", RpcTarget.All, loadIndex);
    }

    public override void OnConnectedToMaster()
	{
		uiController.EnableStartButton();
	}
	
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("Tried to join a room and failed");
		
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("Joined a room");

        SpawnThings();

		roomManager.DestroyAudioListener();
		uiController.DisableStartButton();
		
		if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
			roomManager.PlayMusic();
	}
	
	public override void OnPlayerEnteredRoom(Player player)
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
			roomManager.PlayMusic();
	}
	
	public override void OnPlayerLeftRoom(Player player)
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
			roomManager.StopMusic();
	}

    public void Play()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
			uiController.DisableStartButton();
            Connect();

            return;
        }

        PhotonNetwork.JoinRandomRoom();
    }
}
