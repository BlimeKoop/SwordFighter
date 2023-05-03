using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeArmIKTargetController : MonoBehaviour
{
	public PlayerSwordController swordController;
	
	private void Update()
	{		
		transform.position = swordController.GetRigidbody().position;
		transform.rotation = swordController.GetRigidbody().rotation;
	}

	public void SetSwordController(PlayerSwordController swordController) { this.swordController = swordController; }
}
