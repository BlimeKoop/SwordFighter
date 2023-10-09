using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Alteruna;

public class PlayerInputController
{
	public Alteruna.Avatar avatar;
	
	PlayerController playerController;
	PlayerInputActions playerInputActions;
	
	[HideInInspector]
	public Vector2 movementInput;

	private Vector2 swingInput;
	
	[HideInInspector] public Vector2 storedSwingInput, swingInputActive;
	
	private float SwingInputMult = 0.1f;
	
	private float swingZeroDelay = 0.15f;
	private float swingZeroTimer;
	
	private int consecutiveSwingZeroCount = 6;
	private int consecutiveSwingZeroCounter;
	
	private bool paused;
	
	[HideInInspector]
	public bool sharpAngleChange;
	
    public void Initialize(PlayerController playerController)
    {
		this.playerController = playerController;
		
        playerInputActions = new PlayerInputActions();
		var playerMap = playerInputActions.PlayerMap;
		
		playerMap.Move.performed += context => RecordMoveInput();
		playerMap.Move.canceled += context => RecordMoveInput();
		playerMap.Swing.performed += context => RecordSwingInput();
		playerMap.Block.performed += context => playerController.Block();
		playerMap.Block.canceled += context => playerController.StopBlock();
		playerMap.TogglePause.performed += context => TogglePause();
		playerMap.UnPause.performed += context => UnPause();
		playerMap.Stab.performed += context => playerController.StartStab();
		playerMap.Stab.canceled += context => playerController.Stab();
    }
	
	public void EnableInput()
	{
		playerInputActions.Enable();
	}
	
	private void RecordMoveInput()
	{
		movementInput = playerInputActions.PlayerMap.Move.ReadValue<Vector2>();
	}

	public void RecordSwingInput()
	{
		swingInput = playerInputActions.PlayerMap.Swing.ReadValue<Vector2>() * SwingInputMult;
	}

	public void StoreSwingInput()
	{
		storedSwingInput = swingInput;
	}

	public Vector2 GetSwingInput()
	{
		return swingInput;
	}

	private void TogglePause()
	{
		paused = !paused;
	}
	
	private void UnPause()
	{
		paused = false;
	}
	
    public void DoUpdate()
    {
		ZeroSwingInput();
    }
	
	private void ZeroSwingInput()
	{
		consecutiveSwingZeroCounter++;
		
		if (playerInputActions.PlayerMap.Swing.ReadValue<Vector2>().magnitude > 0.0f)
			consecutiveSwingZeroCounter = 0;
		
		swingZeroTimer += Time.deltaTime;
		
		if (consecutiveSwingZeroCounter < consecutiveSwingZeroCount)
			swingZeroTimer = 0;
		
		if (swingZeroTimer >= swingZeroDelay)
			swingInput = Vector2.zero;	
	}
	
	public Vector3 MoveDirection() {
		Vector2 movementInput = GetMovementInput();
		
		Transform camera = playerController.camera;

		return (
		Vectors.FlattenVector(camera.right) * movementInput.x +
		Vectors.FlattenVector(camera.forward) * movementInput.y);
	}
	
	public Vector2 GetMovementInput() { return movementInput; }
}
