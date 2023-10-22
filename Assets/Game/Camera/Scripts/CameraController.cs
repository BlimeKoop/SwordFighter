using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	CameraInputController inputController;
	
	public GameObject target;
	public CameraPivotController pivotController;
	
	private bool directionChange;
	
	[HideInInspector] public PlayerController playerController;

    public void Initialize(PlayerController playerController)
    {
		this.playerController = playerController;
		
		pivotController = InitializeCameraPivotController(pivotController);
		inputController = InitializeCameraInputController();
		inputController.Initialize(this);
		
		transform.parent = pivotController.transform;
    }

	private CameraPivotController InitializeCameraPivotController(CameraPivotController controller)
	{		
		controller.Initialize(target);
		
		return controller;
	}

	private CameraInputController InitializeCameraInputController()
	{
		CameraInputController cicReturn = GetComponent<CameraInputController>();
		
		if (cicReturn == null)
			cicReturn = gameObject.AddComponent<CameraInputController>();
		
		return cicReturn;
	}
	
	public void ChangeDirection()
	{
		if (pivotController == null)
			return;
		
		// if (inputController.timeSinceInput > 2f)
			// return;
		
		pivotController.ChangeDirection(inputController.aimInputActive);
		playerController.CancelStab();
	}

	public void Rotate(float degrees)
	{
		pivotController.Rotate(degrees);
	}

    public void Disable()
    {
		pivotController.enabled = false;
		this.enabled = false;
    }
	
	public void ToggleDirectionChange()
	{
		directionChange = !directionChange;
	}
}
