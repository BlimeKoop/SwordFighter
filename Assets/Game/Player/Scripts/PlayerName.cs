using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerName : MonoBehaviour
{
    private TextMeshProUGUI nameTextUI;
    private TextMeshPro nameText;
	
	private void Start()
	{
		PhotonView photonView = GetComponentInParent<PhotonView>();
		
		nameTextUI = GetComponent<TextMeshProUGUI>();
		nameText = GetComponent<TextMeshPro>();
		
		if (photonView.Controller.NickName == null || photonView.Controller.NickName == "")
			return;
		
		if (nameTextUI != null)
			nameTextUI.text = photonView.Controller.NickName;
		
		if (nameText != null)
			nameText.text = photonView.Controller.NickName;
	}
}
