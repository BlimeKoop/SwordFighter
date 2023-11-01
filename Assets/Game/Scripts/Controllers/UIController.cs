using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UIController : MonoBehaviour
{
	private static PhotonView photonView;
	
	public Transform canvas;
	
	private PlayerUIController playerUI;
	
	public GameObject _startButton;		private static GameObject startButton;
	public GameObject _winText;			private static GameObject winText;
	public GameObject _loseText;		private static GameObject loseText;
	public GameObject _pauseMenu;		private static GameObject pauseMenu;
	public GameObject _background;		private static GameObject background;
	
	private void Awake()
	{
		startButton = _startButton;
		winText = _winText;
		loseText = _loseText;
		pauseMenu = _pauseMenu;
		background = _background;
		
		photonView = GetComponent<PhotonView>();
		canvas.gameObject.SetActive(true);
		
		playerUI = canvas.GetComponentInChildren<PlayerUIController>();
	}
	
	public void OnExitButtonClicked()
	{
		NetworkSceneManager.LoadLevel("Menu");
	}
	
	public void OnBackButtonClicked()
	{
		GameManager.UnPause();
	}
	
	public static void EnableStartButton()
	{
		startButton.SetActive(true);
	}
	
	public static void DisableStartButton()
	{
		startButton.SetActive(false);
	}
	
	public static void EnableWinText()
	{
		DisableLoseText();
		
		winText.SetActive(true);
	}
	
	public static void DisableWinText()
	{
		winText.SetActive(false);
	}
	
	public static void EnableLoseText()
	{
		DisableWinText();
		
		loseText.SetActive(true);
	}
	
	public static void DisableLoseText()
	{
		loseText.SetActive(false);
	}

	public static void EnablePauseMenu()
	{
		pauseMenu.SetActive(true);
	}

	public static void DisablePauseMenu()
	{
		pauseMenu.SetActive(false);
	}

	public static void EnableBackground()
	{
		background.SetActive(true);
	}

	public static void DisableBackground()
	{
		background.SetActive(false);
	}
}
