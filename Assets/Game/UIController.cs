using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
	private GameObject startButton;
	private GameObject winText;
	
	private void Awake()
	{
		startButton = transform.Find("Button").gameObject;
		winText = transform.Find("Win Text").gameObject;
		
		DisableWinText();
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
}
