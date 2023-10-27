using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[HideInInspector] public PlayerController playerController;
	
	CameraInputController inputController;
	
	public Transform target;
	public CameraPivotController pivot;
	
	private bool orbit, enabled;
	
	private void OnEnable()
	{
		enabled = true;
		pivot.enabled = true;
	}
	
	private void OnDisable()
	{
		enabled = false;
		pivot.enabled = false;
		
		if (inputController != null)
			inputController.DisableInput();
	}

    public void Initialize(PlayerController playerController)
    {
		this.playerController = playerController;
		
		pivot = InitializeCameraPivotController(pivot);
		inputController = InitializeCameraInputController();
		inputController.Initialize(this);
		
		transform.parent = pivot.transform;
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
	
	public void DoUpdate()
	{
		if (!enabled)
			return;
		
		inputController.DoUpdate();
		
		if (orbit)
			pivot.Rotate(inputController.orbitInput * 18f);
	}
	
	public void ChangeDirection()
	{
		if (pivot == null)
			return;
		
		pivot.ChangeDirection(inputController.aimInputActive);
		playerController.CancelStab();
	}

	public void AutoRotate(float degrees)
	{
		if (orbit)
			return;
		
		pivot.SetAutoRotateDegrees(degrees);
	}
	
	public void Orbit()
	{
		orbit = true;
	}

	public void StopOrbit()
	{
		orbit = false;
	}
	
	private void OnDestroy()
	{
		if (inputController == null)
			return;
		
		inputController.DisableInput();
	}
}
