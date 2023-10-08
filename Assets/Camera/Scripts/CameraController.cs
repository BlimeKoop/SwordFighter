using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	CameraInputController cameraInputController;
	
	public GameObject target;
	public CameraPivotController pivotController;
	
	[HideInInspector] public PlayerController playerController;

    public void Initialize(PlayerController playerController)
    {
		this.playerController = playerController;
		
		pivotController = InitializeCameraPivotController(pivotController);
		cameraInputController = InitializeCameraInputController();
		cameraInputController.Initialize(this);
		
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
		pivotController.ChangeDirection(playerController.movement);
	}
	
	private Vector3 NonPlayerAimDirection(CameraInputActions cameraInputActions) {
		Vector3 aimInput = cameraInputActions.CameraMap.Aim.ReadValue<Vector2>();
		float inputX = aimInput.x;
		
		if (aimInput.sqrMagnitude == 0)
			return transform.forward;
		
		Vector3 rightFlat = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
		Vector3 forwardFlat = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;	
		
		Vector3 directionR = (rightFlat * inputX + forwardFlat * (1.0f - Mathf.Abs(inputX))).normalized;
		
		return directionR;
	}

	public void Rotate(float degrees)
	{
		pivotController.Rotate(degrees);
	}
}
