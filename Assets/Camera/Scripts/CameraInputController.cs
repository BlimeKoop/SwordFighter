using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputController : MonoBehaviour
{
	CameraController cameraController;
	CameraInputActions inputActions;

    public void Initialize(CameraController cameraController)
    {
		this.cameraController = cameraController;

		if (!cameraController.playerController.avatar.IsOwner)
			return;
		
        inputActions = new CameraInputActions();

		MapInputCallbacks();
		
		StartCoroutine(EnableInput(0.2f));
    }
	
	private void MapInputCallbacks()
	{
		inputActions.CameraMap.ChangeDirection.performed += (
			context => cameraController.ChangeDirection());
	}
	
	private IEnumerator EnableInput(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		inputActions.Enable();
	}
}
