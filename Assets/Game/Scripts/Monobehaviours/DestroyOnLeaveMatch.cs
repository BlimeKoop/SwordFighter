using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class DestroyOnLeaveMatch : MonoBehaviourPunCallbacks
{
    public override void OnLeftRoom()
	{
		if (SceneManager.GetActiveScene().name == "Menu")
			Destroy(gameObject);
	}
}
