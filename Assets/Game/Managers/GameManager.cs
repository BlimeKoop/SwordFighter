using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
	public GameObject networkSceneManagerPrefab;
	public GameObject networkObjectManagerPrefab;
	
	public static bool paused;
	
	private void Awake()
	{
		if (FindObjectOfType<NetworkSceneManager>() == null)
			Instantiate(networkSceneManagerPrefab);
		
		if (FindObjectOfType<NetworkObjectManager>() == null)
			Instantiate(networkObjectManagerPrefab);
	}
	
	public static void TogglePause()
	{
		paused = !paused;

		if (paused)
			UIController.EnablePauseMenu();
		else
			UIController.DisablePauseMenu();
		
		Cursor.visible = paused;
		Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
	}
	
	public static void UnPause()
	{
		paused = false;
		
		UIController.DisablePauseMenu();
		
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	public static void ExitGame()
	{
		Application.Quit();
	}
}
