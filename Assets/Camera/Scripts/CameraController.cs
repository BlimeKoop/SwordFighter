using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	CameraInputController cameraInputController;
	
	public GameObject pivot;
	private CameraPivotController cameraPivotController;
	
	private float directionChangeTimer;

    void Start()
    {
		cameraPivotController = InitializeCameraPivotController();
		cameraInputController = InitializeCameraInputController();
		cameraInputController.Initialize(this);
		
		transform.parent = pivot.transform;
    }

	private CameraPivotController InitializeCameraPivotController()
	{
		CameraPivotController cpcReturn = pivot.GetComponent<CameraPivotController>();
		
		if (cpcReturn == null)
			cpcReturn = pivot.AddComponent<CameraPivotController>();
		
		cpcReturn.Initialize();
		
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
		
		Vector2 aimInput = cameraInputActions.CameraMap.Aim.ReadValue<Vector2>();
		
		if (aimInput.sqrMagnitude == 0)
			return;
		
		Vector3 rightFlat = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
		Vector3 forwardFlat = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;	
		
		Vector3 aimDirection = rightFlat * aimInput.x + forwardFlat * aimInput.y;
		
		cameraPivotController.SetTargetDirection(aimDirection);
	}
}
