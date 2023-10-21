using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
	public Transform canvas;
	
	public GameObject startButton;
	public GameObject winText;
	public GameObject loseText;
	public GameObject background;
	
	private void Awake()
	{
		canvas.gameObject.SetActive(true);
	}
	
	public void EnableStartButton()
	{
		startButton.SetActive(true);
	}
	
	public void DisableStartButton()
	{
		startButton.SetActive(false);
	}
	
	public void EnableWinText()
	{
		winText.SetActive(true);
	}
	
	public void DisableWinText()
	{
		winText.SetActive(false);
	}
	
	public void EnableLoseText()
	{
		loseText.SetActive(true);
	}
	
	public void DisableLoseText()
	{
		loseText.SetActive(false);
	}

	public void EnableBackground()
	{
		background.SetActive(true);
	}

	public void DisableBackground()
	{
		background.SetActive(false);
	}
}
