using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
	PlayerController playerController;
	PlayerInputActions playerInputActions;
	
	private Vector2 movementInput;
	
	private Vector2 swingInput;
	private Vector2 swingInputStore;
	private Vector2 swingInputActive;
	
	private float SwingInputMult = 0.1f;
	
	private int consecutiveSwingInputCount;
	private int ConsecutiveSwingZeroCount = 6;
	
	private bool paused;
	private bool lockPause;
	
    void Start()
    {
		playerController = GetComponent<PlayerController>();
        playerInputActions = new PlayerInputActions();
		
		var playerMap = playerInputActions.PlayerMap;
		
		playerMap.Swing.performed += context => UpdateSwingInput();
		playerMap.Block.performed += context => playerController.Block();
		playerMap.Block.canceled += context => playerController.StopBlock();
		playerMap.Pause.performed += context => Pause();
		playerMap.UnPause.performed += context => UnPause();
		playerMap.Stab.performed += context => playerController.StartStab();
		playerMap.Stab.canceled += context => playerController.Stab();

		StartCoroutine(EnableInput(0.2f));
    }
	
	private void Pause()
	{
		paused = true;
		lockPause = true;
	}
	
	private void UnPause()
	{
		if (lockPause)
			return;
		
		paused = false;
	}
	
	public void UpdateSwingInput()
	{
		if (paused)
		{
			swingInputStore = Vector2.zero;
			swingInput = Vector2.zero;
			
			lockPause = false;
			
			return;
		}
		
		swingInputStore = swingInput;
		swingInput = playerInputActions.PlayerMap.Swing.ReadValue<Vector2>() * SwingInputMult;
		
		// Debug.Log(swingInput.magnitude);
	}

	private IEnumerator EnableInput(float delay)
	{
		yield return new WaitForSeconds(delay);

		playerInputActions.Enable();
	}

    void Update()
    {
		if (paused)
		{
			movementInput = Vector2.zero;
			
			return;
		}
		
		movementInput = playerInputActions.PlayerMap.Move.ReadValue<Vector2>();
        consecutiveSwingInputCount = playerInputActions.PlayerMap.Swing.ReadValue<Vector2>().sqrMagnitude == 0.0f ? consecutiveSwingInputCount + 1 : 0;

		if (consecutiveSwingInputCount >= ConsecutiveSwingZeroCount)
			swingInput *= 0;
		
		if (swingInput.magnitude > 0.0f)
			swingInputActive = swingInput;
    }
	
	public Vector3 SwingDirection() {
		Vector2 movementInput = GetMovementInput();
		
		Transform cam = playerController.GetCamera();

		return (
		Vectors.FlattenVector(cam.right) * movementInput.x +
		Vectors.FlattenVector(cam.forward) * movementInput.y);
	}
	
	public Vector2 GetMovementInput() { return movementInput; }
	public Vector2 GetSwingInput() { return swingInput; }
	public Vector2 GetSwingInputStore() { return swingInputStore; }
	public Vector2 GetSwingInputActive() { return swingInputActive; }
}
