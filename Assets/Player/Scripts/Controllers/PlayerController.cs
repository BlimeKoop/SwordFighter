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
	
	public Transform rig;
	public Transform swordRig;
	
	public Alteruna.Avatar avatar;
	
	public Transform camera;
	[HideInInspector] public CameraController cameraController;
	
	[SerializeField] private Transform _sword;
	[HideInInspector] public Transform sword;
	
	public float moveSpeed = 8.0f;
	public float swingSpeed = 9.0f;

	public float groundDetectionRadius = 0.6f;
	public float groundStepUpDistance = 0.4f;

	public RaycastHit groundHit;

	[HideInInspector] public Vector3 movement; 
	
	[HideInInspector] public bool block, alignStab, stab, holdStab;
	
	public float SwordHoldDistance = 0.5f;
	
	private float StabHoldDuration = 0.5f;
	[HideInInspector] public float stabHoldTimer;

    void Start()
    {
		if (!avatar.IsOwner)
		{
			camera.gameObject.SetActive(false);
			GetComponent<Animator>().enabled = false;
			return;
		}
		
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;

		camera = Camera.main.transform;
		cameraController = camera.GetComponent<CameraController>();
		cameraController.Initialize(this);
		
		input = GetComponent<PlayerInput>();
		
		inputController = new PlayerInputController();
		collisionController = new PlayerCollisionController();
		animationController = new PlayerAnimationController();
		
		inputController.Initialize(this); StartCoroutine(EnableInput(0.2f));
		animationController.Initialize(this);
		
		sword = new GameObject($"{gameObject.name} Sword").transform;
		
		swordController = sword.gameObject.AddComponent<PlayerSwordController>();
		swordController.Initialize(this, inputController, animationController);
		
		physicsController = new PlayerPhysicsController();
		physicsController.Initialize(this, inputController, animationController);
		
	}
	
	private IEnumerator EnableInput(float delay)
	{
		yield return new WaitForSeconds(delay);

		inputController.EnableInput();
	}
	
	void FixedUpdate()
	{		
		if (!avatar.IsOwner)
			return;
		
		physicsController.MoveRigidbody(movement);
		physicsController.RotateRigidbody();
	
		swordController.DoFixedUpdate();
	}

    void Update()
    {
		if (!avatar.IsOwner)
			return;
		
		inputController.DoUpdate();
		animationController.DoUpdate();
		
		movement = inputController.SwingDirection() * moveSpeed;
		
		animationController.SetAnimatorFloat(
			"Speed",
			physicsController.rigidbodySync.velocity.magnitude * 0.35f);
			
		animationController.SetAnimatorLayerWeight(
			1,
			physicsController.rigidbodySync.velocity.magnitude / moveSpeed);
		
		if (swordController.inputAngleChange > 90f)
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
	
	private void OnCollisionEnter(Collision col)
	{
		physicsController.Collide(col);
	}
	
	private void OnCollisionExit (Collision col)
	{
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
	
	public Transform GetSwordModel() { return _sword.transform; }
	
	public Vector3 ToSword() {
		return swordController.GetComponent<Rigidbody>().position - transform.position;
	}
	
	public Vector3 ArmToSword() {
		return swordController.GetComponent<Rigidbody>().position - animationController.rightArmBone.position;
	}

	public Vector3 ApproximateArmToSword() {
		return swordController.GetComponent<Rigidbody>().position - animationController.ApproximateArmPosition();
	}
	
	public Vector3 ForeArmToSword() {
		return swordController.GetComponent<Rigidbody>().position - animationController.rightForeArmBone.position;
	}
	
	public Vector3 ChestToSword() {
		return swordController.GetComponent<Rigidbody>().position - animationController.chestBone.position;
	}

	public Vector3 ApproximateChestToSword() {
		return swordController.GetComponent<Rigidbody>().position - animationController.ApproximateChestPosition();
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
