using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeArmIKTargetController : ArmIKTargetController
{
	public PlayerSwordController swordController;
	
	public override Vector3 CalculatePosition()
	{
		return swordController.GetRigidbody().position;		
	}

	public override Quaternion CalculateRotation()
	{
		return transform.rotation = swordController.GetRigidbody().rotation;
	}

	public void SetSwordController(PlayerSwordController swordController) { this.swordController = swordController; }
}
