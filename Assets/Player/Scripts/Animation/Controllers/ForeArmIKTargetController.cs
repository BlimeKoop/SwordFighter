using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeArmIKTargetController : MonoBehaviour
{
	public PlayerSwordController swordController;
	
	public void DoUpdate()
	{
		transform.position = CalculatePosition();
	}
	
	public Vector3 CalculatePosition()
	{
		return swordController.transform.position;		
	}

	public void SetSwordController(PlayerSwordController swordController)
	{ this.swordController = swordController; }
}
