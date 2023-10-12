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
	
	private bool initialized, dead;

    void Start()
    {		
		// Cursor.visible = false;
		// Cursor.lockState = CursorLockMode.Locked;

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
		collisionController = new PlayerCollisionController();
		
		swordController.Initialize(this, inputController, animationController);
		physicsController.Initialize(this, inputController, animationController);
		inputController.Initialize(this); StartCoroutine(StartInput(0.2f));

		initialized = true;
	}
	
	private void InitializeSword()
	{
		swordModel = sword;
		swordModel.GetComponentInChildren<Collider>().gameObject.layer = Collisions.SwordLayer;
		
		sword = transform.parent.Find("Player Sword");
		sword.position = animationController.rightHandBone.position;
		sword.rotation = Quaternion.LookRotation(animationController.rightHandBone.up, -animationController.rightHandBone.forward);
		
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
		StartCoroutine(GetSwingInput());
		StartCoroutine(CheckInputChange());
	}
	
	private IEnumerator GetSwingInput()
	{
		if (inputController.GetSwingInput().sqrMagnitude == 0)
			yield return new WaitUntil(() => inputController.GetSwingInput().sqrMagnitude > 0);
		else
			yield return null;
		
		Vector3 swingInput = inputController.GetSwingInput();
		
		swordController.SetTipInput(swingInput);
		swordController.SetBaseInput(swingInput);

		StartCoroutine(GetSwingInput());
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
		if (!photonView.IsMine || dead)
			return;
		
		physicsController.MoveRigidbody(movement);
		physicsController.RotateRigidbody();
	
		swordController.DoFixedUpdate();
	}

    void Update()
    {
		if (!photonView.IsMine || dead)
			return;
		
		animationController.DoUpdate();
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
		if(dead || !photonView.IsMine)
			return;
		
		cameraController.pivotController.DoLateUpdate();
	}
	
	private void OnCollisionEnter(Collision col)
	{
		if(dead || !photonView.IsMine || !initialized)
			return;
		
		if (col.transform == sword)
			return;
		
		physicsController.Collide(col);
	}
	
	private void OnCollisionExit (Collision col)
	{
		if(dead || !photonView.IsMine)
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
