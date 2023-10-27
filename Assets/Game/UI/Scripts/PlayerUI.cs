using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerUI : MonoBehaviour
{
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI livesText;
	
	public void SetName(string setTo)
	{
		nameText.text = setTo;
	}
	
    public void SetLives(int setTo)
	{
		livesText.text = "";
		
		for(int i = 0; i < setTo; i++)
			livesText.text += ".";
	}
}
