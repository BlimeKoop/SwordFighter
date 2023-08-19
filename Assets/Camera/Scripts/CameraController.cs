using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	CameraInputController cameraInputController;
	
	public GameObject target;
	private Transform pivot;
	
	private CameraPivotController cameraPivotController;
	
	private float directionChangeTimer;
	
	private bool followingPlayer;
	private PlayerController playerController;

    void Start()
    {
		cameraPivotController = InitializeCameraPivotController();
		cameraInputController = InitializeCameraInputController();
		cameraInputController.Initialize(this);
		
		transform.parent = pivot.transform;
		
		playerController = target.GetComponent<PlayerController>();
		followingPlayer = playerController != null;
    }

	private CameraPivotController InitializeCameraPivotController()
	{
		pivot = new GameObject($"{gameObject.name} Pivot").transform;
		
		CameraPivotController cpcReturn = pivot.gameObject.AddComponent<CameraPivotController>();
		
		cpcReturn.Initialize(target);
		
		return cpcReturn;
	}

	private CameraInputController InitializeCameraInputController()
	{
		CameraInputController cicReturn = GetComponent<CameraInputController>();
		
		if (cicReturn == null)
			cicReturn = gameObject.AddComponent<CameraInputController>();
		
		return cicReturn;
	}
	
	public void ChangeDirection(CameraInputActions cameraInputActions)
	{
		if (!cameraPivotController.ForwardAligned())
			return;
		
		if (!followingPlayer)
			cameraPivotController.SetTargetDirection(NonPlayerAimDirection(cameraInputActions)); 
		else
			cameraPivotController.SetTargetDirection(
			Vectors.FlattenVector(playerController.GetSwordObject().forward).normalized);
			
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
}
