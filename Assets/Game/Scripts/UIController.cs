using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
	public Transform canvas;
	
	private GameObject startButton;
	private GameObject winText;
	private GameObject loseText;
	
	private void Awake()
	{
		canvas.gameObject.SetActive(true);
		
		startButton = canvas.Find("Button").gameObject;
		winText = canvas.Find("Win Text").gameObject;
		loseText = canvas.Find("Lose Text").gameObject;
		
		DisableWinText();
		DisableLoseText();
		DisableStartButton();
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
}
