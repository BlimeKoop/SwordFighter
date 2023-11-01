using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using DynamicMeshCutter;

public class MatchManager : MonoBehaviourPunCallbacks
{
	public static PhotonView photonView;

	public PhotonView playerPrefab;
	public PhotonView playerDataPrefab;
	
	private Transform spawnPoints;
	
	public int _deathPlaneHeight = -50;
	
	public static int deathPlaneHeight;
	
	private PhotonView playerDataView;
	private PlayerData playerData;
	
	private static MatchManager objectRef;
	
	bool waitingForThingsToSpawn;
	
	private void Awake()
	{
		DontDestroyOnLoad(this);
		
		photonView = GetComponent<PhotonView>();
	}
	
	private void Start()
	{
		spawnPoints = GameObject.Find("Spawn Points").transform;
		deathPlaneHeight = _deathPlaneHeight;
		
		new GameObject("Death Plane").AddComponent<DeathPlane>().Initialize(_deathPlaneHeight);

		if (PhotonNetwork.IsMasterClient)
		{
			playerDataView = PhotonNetwork.Instantiate(
				playerDataPrefab.gameObject.name, new Vector3(), new Quaternion()).GetComponent<PhotonView>();
				
			playerData = GameObject.FindObjectOfType<PlayerData>();
		}
		else
			StartCoroutine(FindPlayerData());

		objectRef = this;
		StartCoroutine(SetUpRoomWhenReady());
	}
	
	private IEnumerator FindPlayerData()
	{
		waitingForThingsToSpawn = true;
		
		yield return new WaitWhile(() => GameObject.FindObjectOfType<PlayerData>() == null);
		
		playerData = GameObject.FindObjectOfType<PlayerData>();
		playerDataView = playerData.GetComponentInParent<PhotonView>();
		
		waitingForThingsToSpawn = false;
	}
	
	private IEnumerator SetUpRoomWhenReady()
	{
		yield return new WaitWhile(() => waitingForThingsToSpawn);
		
		SetUpRoom();
		OnPlayerEnteredRoom(PhotonNetwork.LocalPlayer);
	}
	
	public void Connect()
	{
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnPlayerEnteredRoom(Player player)
	{
		if (SceneManager.GetActiveScene().name == "Multiplayer")
			OnPlayerJoinedMatch(player);
	}
	
	public override void OnPlayerLeftRoom(Player player)
	{
		if (SceneManager.GetActiveScene().name == "Multiplayer")
			OnPlayerLeftMatch(player);
		
		if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
			MatchAudioManager.StopMusic();
	}
	
	public void OnPlayerJoinedMatch(Player player)
	{
		if (player == PhotonNetwork.LocalPlayer)
			playerDataView.RPC("AddPlayer", RpcTarget.AllBufferedViaServer, player);
	}
	
	public void OnPlayerLeftMatch(Player player)
	{
		playerData.RemovePlayer(player);
	}
	
	public static void SetUpRoom()
	{
		UIController.DisableStartButton();

		objectRef.InstantiatePlayer();
		
		if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
			MatchAudioManager.PlayMusic();
	}
	
    private void InstantiatePlayer()
    {
        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.childCount;

        GameObject player = PhotonNetwork.Instantiate(
			playerPrefab.gameObject.name, spawnPoints.GetChild(index).position, spawnPoints.GetChild(index).rotation);
    }
	
	public static void ResetObjects()
    {
		if (PlayerData.MatchFinished())
			return;
		
		photonView.RPC("ResetObjectsRPC", RpcTarget.All);
	}
	
	[PunRPC]
	public void ResetObjectsRPC()
	{
		foreach(var playerInputController in FindObjectsOfType<PlayerInputController>())
			playerInputController.DisableInput();
			
		PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
		NetworkSceneManager.LoadLevel(SceneManager.GetActiveScene().name);
		
		objectRef.StopCoroutine(objectRef.CallOnObjectReset());
		objectRef.StartCoroutine(objectRef.CallOnObjectReset());
	}

	private IEnumerator CallOnObjectReset()
	{
		yield return new WaitUntil (() => PhotonNetwork.LevelLoadingProgress == 1);
		
		objectRef.OnObjectReset();
	}
	
	private void OnObjectReset()
	{
		spawnPoints = GameObject.Find("Spawn Points").transform;
		
		SetUpRoom();
		OnPlayerEnteredRoom(PhotonNetwork.LocalPlayer);
	}
	
	public static void EndMatch()
	{
		Camera[] cameras = FindObjectsOfType<Camera>();
		
		for(int i = 0; i < cameras.Length; i++)
		{
			Camera cam = cameras[i];

			cam.rect = new Rect(
				i > 0 ? (i / (float) (cameras.Length)) : 0,
				0,
				1 / (float) cameras.Length,
				1);
			
			SetUpPostMatchCamera(cam);

		}
	}
	
	private static void SetUpPostMatchCamera(Camera camera)
	{
		camera.enabled = true;
		
		CameraController controller = camera.GetComponent<CameraController>();
		controller.enabled = false;
		
		Orbit orbit = camera.gameObject.AddComponent<Orbit>();
		orbit.target = controller.target;
		orbit.pivot = controller.pivot.transform;
		orbit.height = 4f;
		
		LookAt lookAt = camera.gameObject.AddComponent<LookAt>();
		lookAt.target = controller.pivot.transform;
		
		controller.pivot.GetComponent<CameraPivotController>().enabled = false;
		controller.pivot.gameObject.AddComponent<Spin>();
		
		controller.pivot.transform.position = controller.target.position + Vector3.up * 3f;
		
		camera.transform.parent = null;
		camera.nearClipPlane = 3f;
	}
}
