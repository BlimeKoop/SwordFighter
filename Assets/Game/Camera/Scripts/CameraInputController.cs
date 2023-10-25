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
	
	[HideInInspector]
	public float orbitInput, lastOrbitInput;
	
	private bool waitForOrbitInput, orbit, holdOrbit;
	
	private int consecutiveZeroCount = 3;
	private int consecutiveZeroCounter;
	
	private float orbitStopTimer;
	private float OrbitStopDelay = 0f;
	private int orbitTapCount = 0;

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
			
		inputActions.CameraMap.OrbitPress.performed += (
			context => PressOrbit());
			
		inputActions.CameraMap.OrbitPress.canceled += (
			context => UnpressOrbit());

		inputActions.CameraMap.OrbitHold.performed += (
			context => HoldOrbit());
			
		inputActions.CameraMap.OrbitHold.canceled += (
			context => StopHoldOrbit());
			
		// inputActions.CameraMap.OrbitTap.performed += (
			// context => TapOrbit());
		
		inputActions.CameraMap.OrbitInput.performed += (
			context => SetOrbitInput());
			
		inputActions.CameraMap.OrbitStop.performed += (
			context => StopOrbit());
			
		inputActions.CameraMap.AimInput.performed += (
			context => SetAimInput());
			
		inputActions.CameraMap.AimInput.canceled += (
			context => SetAimInput());
	}
	
	private void PressOrbit()
	{
		StartOrbit();
	}
	
	private void UnpressOrbit()
	{
		if (!holdOrbit)
			StopOrbit();
	}
	
	private void HoldOrbit()
	{
		holdOrbit = true;
	}
	
	private void StopHoldOrbit()
	{
		if (holdOrbit)
		{
			holdOrbit = false;
			StopOrbit();
		}
	}
	
	private void TapOrbit()
	{
		orbitTapCount++;
		
		if (orbit)
			StopOrbit();
	}

	private void StartOrbit()
	{
		orbit = true;
		waitForOrbitInput = true;
		
		cameraController.Orbit();
		StartCoroutine(ZeroOrbitInput());
	}
	
	private void StopOrbit()
	{
		if (cameraController.gameObject.active)
			StartCoroutine(StopOrbitCR());
	}
	
	private IEnumerator StopOrbitCR()
	{
		orbitStopTimer = 0;
		
		yield return new WaitUntil(() => orbitStopTimer >= OrbitStopDelay);
		
		orbit = false;
		orbitInput = 0;
		orbitTapCount = 0;
		
		cameraController.StopOrbit();
		StopCoroutine(ZeroOrbitInput());
	}
	
	public IEnumerator EnableInput(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		EnableInput();
	}
	
	public void EnableInput()
	{
		inputActions.Enable();
	}
	
	public IEnumerator DisableInput(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		DisableInput();
	}
	
	public void DisableInput()
	{
		inputActions.Disable();
	}
	
	public void DoUpdate()
	{
		if (waitForOrbitInput && Mathf.Abs(orbitInput) > 0.01f)
			waitForOrbitInput = false;
		
		orbitStopTimer += Time.deltaTime;
	}
	
	private void SetAimInput()
	{
		aimInput = inputActions.CameraMap.AimInput.ReadValue<Vector2>();
		
		if (aimInput.sqrMagnitude > 0)
		{
			aimInputActive = aimInput;
		}
	}
	
	public void SetOrbitInput()
	{
		lastOrbitInput = orbitInput;
		orbitInput = inputActions.CameraMap.OrbitInput.ReadValue<float>();
	}
	
	private IEnumerator ZeroOrbitInput()
	{
		if (waitForOrbitInput)
			yield return new WaitWhile(() => waitForOrbitInput);
		
		consecutiveZeroCounter++;
		
		if (Mathf.Abs(inputActions.CameraMap.OrbitInput.ReadValue<float>()) > 0.0f)
			consecutiveZeroCounter = 0;
		
		if (consecutiveZeroCounter >= consecutiveZeroCount)
			orbitInput = 0;
		
		yield return new WaitForSeconds(Time.fixedDeltaTime);

		StartCoroutine(ZeroOrbitInput());
	}
}
