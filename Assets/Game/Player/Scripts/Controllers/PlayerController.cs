using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DynamicMeshCutter;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
	[HideInInspector] public PlayerInputController inputController;
	[HideInInspector] public PlayerAnimationController animationController;
	[HideInInspector] public PlayerPhysicsController physicsController;
	[HideInInspector] public PlayerCollisionController collisionController;
    [HideInInspector] public PlayerSwordController swordController;
	
	[HideInInspector] public PlayerInput input;
	
	public PhotonView photonView;
	
	public Transform rig;
	public Transform swordRig;
	public Transform model;
	
	public Transform camera;
	[HideInInspector] public CameraController cameraController;

    public Transform _sword;
	[HideInInspector] public Transform sword;
	[HideInInspector] public Transform swordModel;
	
	public float walkSpeed = 8.0f;
	public float runSpeed = 10.0f; 
	
	[HideInInspector]
	public float moveSpeed;
	
	// [HideInInspector]
	// public float dashSpeed = 4;
	
	public float _swingSpeed = 8.0f;
	
	[HideInInspector] public float swingSpeed { get { return _swingSpeed * 140f; } }
	
	public float jumpHeight = 4f;

	public float groundDetectionRadius = 0.6f;
	public float groundStepUpDistance = 0.4f;

	public RaycastHit groundHit;

	[HideInInspector] public Vector3 movement, movementActive;
	
	[HideInInspector] public bool crouching, block, alignStab, stab, holdStab, paused, dead;
	
	[HideInInspector]
	public bool disableGround, jumpGrace, jumpInputGrace;
	
	public float SwordHoldDistance = 0.5f;
	
	private const float StabHoldDuration = 0.5f;
	private const float InputAngleSwivelThreshold = 90f;
	
	[HideInInspector] public float stabHoldTimer;
	
	bool aerialCameraRotation;

    void Start()
    {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		animationController = new PlayerAnimationController();
		animationController.Initialize(this);

		swordController = new PlayerSwordController();
		
		InitializeSword();
		
		if (!photonView.IsMine)
		{
			camera.gameObject.SetActive(false);
			// GetComponent<Animator>().enabled = false;
			
			return;
		}
		
		physicsController = new PlayerPhysicsController();

		cameraController = camera.GetComponent<CameraController>();
		cameraController.Initialize(this);
		
		input = GetComponent<PlayerInput>();
		
		inputController = new PlayerInputController();
		collisionController = gameObject.AddComponent<PlayerCollisionController>();
		
		swordController.Initialize(this, inputController, animationController);
		physicsController.Initialize(this);
		inputController.Initialize(this); StartCoroutine(StartInput(0.2f));
		collisionController.Initialize(this);
		
		moveSpeed = walkSpeed;
	}
	
	private void InitializeSword()
	{
		swordModel = _sword;
		swordModel.GetComponentInChildren<Collider>().gameObject.layer = Collisions.SwordLayer;
		
		sword = transform.parent.Find("Player Sword");
		sword.position = animationController.bones["rh"].position;
		sword.rotation = Quaternion.LookRotation(
			animationController.bones["rh"].up, -animationController.bones["rh"].right);
		
		swordController.length = PlayerSword.OrientModelToLength(sword, swordModel);

		swordModel.position += (
			sword.position -
			swordModel.GetComponentInChildren<MeshRenderer>().bounds.center);
			
		swordModel.position += sword.forward * swordController.length * (0.5f - swordController.grabPointRatio);
		swordModel.parent = sword.GetChild(0);
	}
	
	private IEnumerator StartInput(float delay)
	{
		yield return new WaitForSeconds(delay);

		inputController.EnableInput();
		StartCoroutine(SwingInput());
		StartCoroutine(CheckInputChange());
	}
	
	private IEnumerator SwingInput()
	{
		if (inputController.SwingInput().sqrMagnitude == 0)
			yield return new WaitUntil(() => inputController.SwingInput().sqrMagnitude > 0);
		else
			yield return null;
		
		Vector3 swingInput = inputController.SwingInput();
		
		swordController.SetTipInput(swingInput);
		swordController.SetBaseInput(swingInput);

		StartCoroutine(SwingInput());
	}
	
	private IEnumerator CheckInputChange()
	{
		Vector2 inputStore = inputController.SwingInput();
		
		yield return new WaitForSeconds(0.07f);
		
		swordController.CheckInputChange(inputStore);
		
		StartCoroutine(CheckInputChange());
	}
	
	void FixedUpdate()
	{		
		if (!photonView.IsMine || dead || paused)
			return;
		
		physicsController.MoveRigidbody(movement);
		physicsController.Rotate();
	
		swordController.DoFixedUpdate();
	}

    void Update()
    {
		if (!photonView.IsMine)
			return;
		
		if (transform.position.y < RoomManager.deathPlaneHeight)
			inputController.Restart();
		
		if (dead)
			return;
		
		animationController.DoUpdate();
		
		if (paused)
			return;
		
		inputController.DoUpdate();
		collisionController.DoUpdate();
		
		movement = inputController.MoveDirection() * moveSpeed;
		
		if (movement.sqrMagnitude > 0.01f)
			movementActive = movement;
		
		if (swordController.inputAngleChange > InputAngleSwivelThreshold)
		{
			if (!block && !stab)
			{
				animationController.swordArmIKTargetController.Slow();
			}
		}
		
		if (stab && ArmToSword().magnitude >= animationController.ArmLength())
			HoldStab();
		
		if (holdStab && stabHoldTimer > StabHoldDuration)
			StopStab();

		swordController.DoUpdate();
		
		float deg = Vector3.Dot(movement, camera.right);
		
		if (!aerialCameraRotation && collisionController.groundDistance > 5f)
			aerialCameraRotation = true;
		else if (aerialCameraRotation && collisionController.onGround)
			aerialCameraRotation = false;
		
		if (!aerialCameraRotation)
			deg *= 1 + (Mathf.Abs(Vector3.Dot(movement, camera.forward)) * 0.6f);
		else
			deg *= 8.8f;
		
		cameraController.Rotate(deg);
		RotateModel();

		stabHoldTimer += Time.deltaTime;
    }
	
	public void RotateModel()
	{
		model.localRotation = Quaternion.Lerp(
			model.localRotation,
			Quaternion.Euler(0f, 45f * inputController.MovementInput().x, 0f),
			Time.fixedDeltaTime * 2f);
	}

	private void LateUpdate()
	{
		if(dead || !photonView.IsMine || paused)
			return;
		
		cameraController.pivotController.DoLateUpdate();
	}

    public void Block() { block = true; }
	public void StopBlock() { block = false; }
	
	public void StartStab() { alignStab = true; }
	public void CancelStab() { alignStab = false; }
	public void Stab() { if (!alignStab) return; alignStab = false; stab = true; }
	
	public void HoldStab()
	{
		stab = false;
		holdStab = true;
		stabHoldTimer = 0.0f;
		swordController.RecordStabRotation();
	}
	
	public void StopStab() { holdStab = false; }

	public void ToggleCrouch()
	{
		crouching = !crouching;
		
		// This is just so the sword doesn't bonk the head or neck
		collisionController.colliders["h"].isTrigger = crouching;
		collisionController.colliders["n"].isTrigger = crouching;
	}

	public void TryJump()
	{
		if (!jumpGrace && !collisionController.onGround)
		{
			StopCoroutine(JumpInputGracePeriod());
			StartCoroutine(JumpInputGracePeriod());
			
			return;
		}
		
		Jump();
	}
	
	public void Jump()
	{		
		jumpGrace = false;
		
		physicsController.Jump();
		
		StartCoroutine(DisableGround());
	}
	
	public IEnumerator JumpGracePeriod()
	{
		jumpGrace = true;
		
		yield return new WaitForSeconds(0.2f);
		
		jumpGrace = false;
	}
	
	public IEnumerator JumpInputGracePeriod()
	{
		jumpInputGrace = true;
		
		yield return new WaitForSeconds(0.2f);
		
		jumpInputGrace = false;
	}
	
	public void Dash()
	{
		physicsController.Dash(movementActive.normalized);
		
		moveSpeed = runSpeed;
	}
	
	public void StopRunning()
	{
		moveSpeed = walkSpeed;
	}

	public void TogglePause()
	{
		paused = !paused;
		
		Cursor.visible = paused;
		Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
		
		if (paused)
			physicsController.SetFrozen(true);
	}
	
	public void UnPause()
	{
		paused = false;
		physicsController.SetFrozen(false);
		
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

    public void Die()
	{
		if (sword != null)
			sword.GetComponent<Rigidbody>().useGravity = true;
		
		dead = true;
	}
	
	public IEnumerator DisableGround()
	{
		disableGround = true;
		
		yield return new WaitForSeconds(Time.fixedDeltaTime * 8);
		
		disableGround = false;
	}

    public Vector3 ToSword() {
		return swordController.physicsController.Position() - transform.position;
	}
	
	public Vector3 ArmToSword() {
		return swordController.physicsController.Position() - animationController.bones["ra"].position;
	}

	public Vector3 ApproximateArmToSword() {
		return swordController.physicsController.Position() - animationController.ApproximateArmPosition();
	}
	
	public Vector3 ForeArmToSword() {
		return swordController.physicsController.Position() - animationController.bones["rfa"].position;
	}
	
	public Vector3 ChestToSword() {
		return swordController.physicsController.Position() - animationController.bones["c"].position;
	}

	public Vector3 ApproximateChestToSword() {
		return swordController.physicsController.Position() - animationController.bones["c"].position;
	}
	
	public bool SwordHeldVertically(float maxY) {
		return ArmToSword().normalized.y > maxY;
	}
	
	public float HoldDistance()
	{
		return animationController.ArmLength() * (0.3f + (1f - swordController.armBendAmount) * 0.8f);
	}
	
	public float ArmBendAngle() { return animationController.ArmBendAngle(); }
	public float GetStabHoldDuration() { return StabHoldDuration; }
	
	public bool SwordFront() {
		return Math.FloatN1P1(Vector3.Dot(ArmToSword().normalized, camera.forward)) > 0;
	}
	public bool SwordRight() {
		return Math.FloatN1P1(Vector3.Dot(ArmToSword().normalized, camera.right)) > 0;
	}
}
