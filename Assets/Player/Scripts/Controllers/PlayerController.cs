using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DynamicMeshCutter;
using Alteruna;

public class PlayerController : MonoBehaviour
{
	[HideInInspector] public PlayerInputController inputController;
	[HideInInspector] public PlayerAnimationController animationController;
	[HideInInspector] public PlayerPhysicsController physicsController;
	[HideInInspector] public PlayerCollisionController collisionController;
    [HideInInspector] public PlayerSwordController swordController;
	
	[HideInInspector] public PlayerInput input;
	
	public Spawner spawner;
	
	public Transform rig;
	public Transform swordRig;
	
	public Alteruna.Avatar avatar;
	
	public Transform camera;
	[HideInInspector] public CameraController cameraController;
	
	public Transform sword;
	[HideInInspector] public Transform swordModel;
	
	public float moveSpeed = 8.0f;
	public float swingSpeed = 9.0f;

	public float groundDetectionRadius = 0.6f;
	public float groundStepUpDistance = 0.4f;

	public RaycastHit groundHit;

	[HideInInspector] public Vector3 movement; 
	
	[HideInInspector] public bool block, alignStab, stab, holdStab;
	
	public float SwordHoldDistance = 0.5f;
	
	private const float StabHoldDuration = 0.5f;
	private const float InputAngleSwivelThreshold = 90f;
	
	[HideInInspector] public float stabHoldTimer;
	
	private bool dead;

    void Start()
    {		
		// Cursor.visible = false;
		// Cursor.lockState = CursorLockMode.Locked;
		
		if (avatar.Owner != null && !avatar.IsOwner)
		{
			camera.gameObject.SetActive(false);
			// GetComponent<Animator>().enabled = false;
		}

		physicsController = new PlayerPhysicsController();
		animationController = new PlayerAnimationController();
		swordController = new PlayerSwordController();
		
		animationController.Initialize(this);
		
		swordModel = sword;
		sword = new GameObject($"{gameObject.name} Sword").transform;
		sword.position = animationController.rightHandBone.position;
		sword.rotation = animationController.rightHandBone.rotation;
		
		if (avatar.IsOwner)
		{
			cameraController = camera.GetComponent<CameraController>();
			cameraController.Initialize(this);
			
			input = GetComponent<PlayerInput>();
			
			inputController = new PlayerInputController();
			collisionController = new PlayerCollisionController();
			
			swordController.Initialize(this, inputController, animationController);
			physicsController.Initialize(this, inputController, animationController);
			inputController.Initialize(this); StartCoroutine(StartInput(0.2f));
		}
	}
	
	private IEnumerator StartInput(float delay)
	{
		yield return new WaitForSeconds(delay);

		inputController.EnableInput();
		StartCoroutine(RecordSwingInput());
		StartCoroutine(CheckInputChange());
	}
	
	private IEnumerator RecordSwingInput()
	{
		// yield return new WaitWhile(() => inputController.GetSwingInput().sqrMagnitude == 0);
		
		inputController.StoreSwingInput();
		
		float seconds = 0.14f;
		
		while (seconds > 0f)
		{
			seconds -= Time.deltaTime;
			
			if (swordController.inputAngleChange > 50f)
				seconds = 0;
			
			yield return null;
		}
		
		// yield return new WaitWhile(() => inputController.GetSwingInput().sqrMagnitude == 0);
		
		swordController.SetTipInput(inputController.storedSwingInput);
		swordController.SetBaseInput(inputController.GetSwingInput());
		
		StartCoroutine(RecordSwingInput());
		
	}
	
	private IEnumerator CheckInputChange()
	{
		Vector2 inputStore = inputController.GetSwingInput();
		
		yield return new WaitForSeconds(0.07f);
		
		swordController.CheckInputChange(inputStore);
		
		StartCoroutine(CheckInputChange());
	}
	
	void FixedUpdate()
	{		
		if (avatar.Owner != null && !avatar.IsOwner || dead)
			return;
		
		physicsController.MoveRigidbody(movement);
		physicsController.RotateRigidbody();
	
		swordController.DoFixedUpdate();
	}

    void Update()
    {
		if (dead)
			return;
		
		animationController.DoUpdate();
		
		if (avatar.Owner != null && !avatar.IsOwner)
			return;
		
		inputController.DoUpdate();
		
		movement = inputController.MoveDirection() * moveSpeed;
		
		if (swordController.inputAngleChange > InputAngleSwivelThreshold)
		{
			if (!block && !stab)
			{
				// Debug.Log(swordController.GetInputAngleChange());
				animationController.swordArmIKTargetController.Lock();
			}
		}
		
		if (stab && ArmToSword().magnitude >= animationController.GetArmLength())
			HoldStab();
		
		if (holdStab && stabHoldTimer > StabHoldDuration)
			StopStab();

		swordController.DoUpdate();
		
		float deg = Vector3.Dot(movement, camera.right);
		deg *= 1 + (Mathf.Abs(Vector3.Dot(movement, camera.forward)) * 0.6f);
		
		cameraController.Rotate(deg);
		
		groundHit = PlayerDetection.GroundHit(this);

		stabHoldTimer += Time.deltaTime;
    }

	private void LateUpdate()
	{
		if(dead || avatar.Owner != null && !avatar.IsOwner)
			return;
		
		cameraController.pivotController.DoLateUpdate();
	}
	
	private void OnCollisionEnter(Collision col)
	{
		if(dead || avatar.Owner != null && !avatar.IsOwner)
			return;
		
		swordController.collisionController.Collide(col);
		physicsController.Collide(col);
	}
	
	private void OnCollisionExit (Collision col)
	{
		if(dead || avatar.Owner != null && !avatar.IsOwner)
			return;
		
		physicsController.StopColliding();
	}
	
	public void Block() { block = true; }
	public void StopBlock() { block = false; }
	
	public void StartStab() { alignStab = true; }
	public void Stab() { alignStab = false; stab = true; }
	
	public void HoldStab()
	{
		stab = false;
		holdStab = true;
		stabHoldTimer = 0.0f;
		swordController.RecordStabRotation();
	}
	
	public void StopStab() { holdStab = false; }
	
	public void Collide(Collision collision) { physicsController.Collide(collision); }
	public void StopColliding() { physicsController.StopColliding(); }
	
	public void Die()
	{
		dead = true;
	}
	
	public Vector3 ToSword() {
		return swordController.physicsController.RigidbodyPosition() - transform.position;
	}
	
	public Vector3 ArmToSword() {
		return swordController.physicsController.RigidbodyPosition() - animationController.rightArmBone.position;
	}

	public Vector3 ApproximateArmToSword() {
		return swordController.physicsController.RigidbodyPosition() - animationController.ApproximateArmPosition();
	}
	
	public Vector3 ForeArmToSword() {
		return swordController.physicsController.RigidbodyPosition() - animationController.rightForeArmBone.position;
	}
	
	public Vector3 ChestToSword() {
		return swordController.physicsController.RigidbodyPosition() - animationController.chestBone.position;
	}

	public Vector3 ApproximateChestToSword() {
		return swordController.physicsController.RigidbodyPosition() - animationController.ApproximateChestPosition();
	}
	
	public bool SwordHeldVertically(float maxY) {
		return ArmToSword().normalized.y > maxY;
	}
	
	public float GetHoldDistance()
	{
		return animationController.GetArmLength() * (0.3f + (1f - swordController.armBendAmount) * 0.8f);
	}
	
	public float GetArmBendAngle() { return animationController.GetArmBendAngle(); }
	public float GetStabHoldDuration() { return StabHoldDuration; }
	
	public bool SwordFront() {
		return MathFunctions.FloatN1P1(Vector3.Dot(ArmToSword().normalized, camera.forward)) > 0;
	}
	public bool SwordRight() {
		return MathFunctions.FloatN1P1(Vector3.Dot(ArmToSword().normalized, camera.right)) > 0;
	}
}
