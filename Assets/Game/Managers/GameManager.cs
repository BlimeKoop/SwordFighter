using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{	
	public GameObject networkSceneManagerPrefab;
	public GameObject networkObjectManagerPrefab;
	
	private void Awake()
	{
		if (FindObjectOfType<NetworkSceneManager>() == null)
			Instantiate(networkSceneManagerPrefab);
		
		if (FindObjectOfType<NetworkObjectManager>() == null)
			Instantiate(networkObjectManagerPrefab);
	}
	
	public static void ExitGame()
	{
		Application.Quit();
	}
}
