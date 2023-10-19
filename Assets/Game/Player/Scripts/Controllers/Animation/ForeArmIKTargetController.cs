using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeArmIKTargetController : MonoBehaviour
{
	public PlayerController playerController;
	
	public void DoUpdate()
	{
		transform.position = CalculatePosition();
	}
	
	public Vector3 CalculatePosition()
	{
		return playerController.sword.position;		
	}

	public void SetPlayerController(PlayerController playerController)
	{ this.playerController = playerController; }
}
