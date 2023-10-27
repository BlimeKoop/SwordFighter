using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerInputController : MonoBehaviour
{
	public PhotonView photonView;
	
	PlayerController playerController;
	PlayerInputActions playerInputActions;
	
	[HideInInspector]
	public Vector2 movementInput;

	private Vector2 swingInput;
	
	[HideInInspector] public Vector2 storedSwingInput, swingInputActive;
	
	private float SwingInputMult = 0.1f;
	
	private float swingZeroDelay = 0.07f;
	private float swingZeroTimer;
	
	private int consecutiveSwingZeroCount = 2;
	private int consecutiveSwingZeroCounter;
	
	[HideInInspector]
	public bool sharpAngleChange;
	
    public void Initialize(PlayerController playerController)
    {
		this.playerController = playerController;
		
        playerInputActions = new PlayerInputActions();
		var playerMap = playerInputActions.PlayerMap;
		
		playerMap.Move.performed += context => RecordMoveInput();
		playerMap.Move.canceled += context => RecordMoveInput();
		playerMap.Crouch.performed += context => playerController.ToggleCrouch();
		playerMap.Jump.performed += context => playerController.TryJump();
		playerMap.Run.performed += context => playerController.Dash();
		playerMap.Run.canceled += context => playerController.StopRunning();
		playerMap.Swing.performed += context => RecordSwingInput();
		playerMap.Block.performed += context => playerController.Block();
		playerMap.Block.canceled += context => playerController.StopBlock();
		playerMap.TogglePause.performed += context => playerController.TogglePause();
		playerMap.UnPause.performed += context => playerController.UnPause();
		playerMap.Stab.performed += context => playerController.StartStab();
		playerMap.Stab.canceled += context => playerController.Stab();
		playerMap.ResetObjects.performed += context => ResetObjects();
		playerMap.Quit.performed += context => QuitMatch();
    }
	
	private void Update()
	{
		swingZeroTimer += Time.deltaTime;
	}
	
	public void ResetObjects()
	{
        MatchManager.ResetObjects();
	}
	
	public void QuitMatch()
	{
		if (playerInputActions != null)
			playerInputActions.Disable();
		
		NetworkSceneManager.LoadLevel("Menu");
	}

	public void EnableInput()
	{
		playerInputActions.Enable();
	}
	
	public void DisableInput()
	{
		playerInputActions.Disable();
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

	public Vector2 SwingInput()
	{
		return swingInput;
	}
	
	public IEnumerator ZeroSwingInput()
	{
		consecutiveSwingZeroCounter++;
		
		if (playerInputActions.PlayerMap.Swing.ReadValue<Vector2>().magnitude > 0.0f)
			consecutiveSwingZeroCounter = 0;
		
		if (consecutiveSwingZeroCounter < consecutiveSwingZeroCount)
			swingZeroTimer = 0;
		
		if (swingZeroTimer >= swingZeroDelay)
			swingInput = Vector2.zero;	
		
		yield return new WaitForSeconds(Time.fixedDeltaTime * 2);
		
		StartCoroutine(ZeroSwingInput());
	}
	
	public Vector3 MoveDirection() {
		bool onGround = playerController.collisionController.onGround;
		RaycastHit groundHit = playerController.collisionController.groundHit;
		
		Transform camera = playerController.camera;
		Vector2 movementInput = MovementInput();

		return (
		Vectors.FlattenVector(camera.right/*, onGround ? groundHit.normal : Vector3.up*/) * movementInput.x +
		Vectors.FlattenVector(camera.forward/*, onGround ? groundHit.normal : Vector3.up*/) * movementInput.y);
	}
	
	public Vector2 MovementInput() { return movementInput; }
}
