using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputController : MonoBehaviour
{
	CameraController cameraController;
	CameraInputActions inputActions;
	
	[HideInInspector]
	public Vector2 aimInput, aimInputActive;
	
	private bool isInput;
	
	[HideInInspector]
	public float timeSinceInput;

	private void LateUpdate()
	{
		if (!isInput)
			timeSinceInput += Time.deltaTime;
	}

    public void Initialize(CameraController cameraController)
    {
		this.cameraController = cameraController;
		
        inputActions = new CameraInputActions();

		MapInputCallbacks();
		
		StartCoroutine(EnableInput(0.2f));
    }
	
	private void MapInputCallbacks()
	{
		inputActions.CameraMap.ChangeDirection.performed += (
			context => cameraController.ChangeDirection());
			
		inputActions.CameraMap.ToggleDirectionChange.performed += (
			context => cameraController.ToggleDirectionChange());

		inputActions.CameraMap.ToggleDirectionChange.canceled += (
			context => cameraController.ToggleDirectionChange());
			
		inputActions.CameraMap.AimInput.performed += (
			context => SetAimInput());
			
		inputActions.CameraMap.AimInput.canceled += (
			context => SetAimInput());
	}
	
	private IEnumerator EnableInput(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		inputActions.Enable();
	}
	
	private void SetAimInput()
	{
		aimInput = inputActions.CameraMap.AimInput.ReadValue<Vector2>();
		
		if (aimInput.sqrMagnitude > 0)
		{
			aimInputActive = aimInput;
			isInput = true;
			timeSinceInput = 0;
		}
		else
		{
			isInput = false;
		}
	}
}
