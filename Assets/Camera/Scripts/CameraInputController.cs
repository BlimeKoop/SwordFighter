using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputController : MonoBehaviour
{
	CameraController cameraController;
	
	CameraInputActions cameraInputActions;

    public void Initialize(CameraController cameraController)
    {
		this.cameraController = cameraController;

        cameraInputActions = new CameraInputActions();

		MapInputCallbacks();
		
		StartCoroutine(EnableInput(0.2f));
    }
	
	private void MapInputCallbacks()
	{
		cameraInputActions.CameraMap.ChangeDirection.performed += context => cameraController.ChangeDirection(cameraInputActions);
	}
	
	private IEnumerator EnableInput(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		cameraInputActions.Enable();
	}

    void Update()
    {
        
    }
}
