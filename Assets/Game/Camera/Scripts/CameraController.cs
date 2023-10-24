using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	CameraInputController inputController;
	
	public GameObject target;
	public CameraPivotController pivotController;
	
	private bool orbit;
	
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
	
	public void DoUpdate()
	{
		inputController.DoUpdate();
		
		if (orbit)
			pivotController.Rotate(inputController.orbitInput * 18f);
	}
	
	public void ChangeDirection()
	{
		if (pivotController == null)
			return;
		
		pivotController.ChangeDirection(inputController.aimInputActive);
		playerController.CancelStab();
	}

	public void AutoRotate(float degrees)
	{
		if (orbit)
			return;
		
		pivotController.SetAutoRotateDegrees(degrees);
	}

    public void Disable()
    {
		pivotController.enabled = false;
		this.enabled = false;
    }
	
	public void Orbit()
	{
		orbit = true;
	}

	public void StopOrbit()
	{
		orbit = false;
	}
}
