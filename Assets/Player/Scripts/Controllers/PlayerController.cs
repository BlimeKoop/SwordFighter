using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DynamicMeshCutter;

[RequireComponent(typeof(PlayerAnimationController))]

public class PlayerController : MonoBehaviour
{
	private PlayerInputController inputController;
	private PlayerAnimationController animationController;
	private PlayerPhysicsController physicsController;
	private PlayerCollisionController collisionController;
    private PlayerSwordController swordController;
	
	private PlayerInput input;
	
	private Camera _cam;
	private Transform cam;
	
	[SerializeField] private Transform swordObject;
	[SerializeField] private float movementSpeed = 7.0f;

	private Vector3 movement;
	
	private bool block;
	private bool alignStab, stab, holdStab;
	
	private float stabHoldTimer;
	private float StabHoldDuration = 0.4f;

    void Awake()
    {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		
		inputController = GetComponent<PlayerInputController>();

		input = GetComponent<PlayerInput>();

		_cam = Camera.main;
		cam = _cam.transform;

		collisionController = PlayerControllerInitialization.CollisionController(this);
		
		animationController = PlayerControllerInitialization.AnimationController(this);
		animationController.Initialize(this);
		
		physicsController = PlayerControllerInitialization.PhysicsController(this);
		physicsController.Initialize(this, inputController, animationController);
		
		swordController = PlayerControllerInitialization.SwordController(this);
		swordController.Initialize(this, inputController, animationController);
	}

	void FixedUpdate()
	{		
		physicsController.MoveRigidbody(movement);
		physicsController.RotateRigidbody();
	
		swordController.DoFixedUpdate();
	}

    void Update()
    {
		Vector2 movementInput = inputController.GetMovementInput();
		
		Vector3 camRightFlat = new Vector3(cam.right.x, 0f, cam.right.z).normalized;
		Vector3 camForwardFlat = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;		
		
		Vector3 moveDir = camRightFlat * movementInput.x + camForwardFlat * movementInput.y;
		movement = moveDir * movementSpeed * Time.fixedDeltaTime;
		
		Animator a = GetComponent<Animator>();
		
		a.SetFloat("Speed", physicsController.GetRigidbody().velocity.magnitude * 0.35f);
		a.SetLayerWeight(1, physicsController.GetRigidbody().velocity.magnitude / movementSpeed);
		
		if (!block && swordController.GetInputAngleChange() > 50f)
			animationController.LockArmIKTargetController();
		
		if (stab && GetSwordDistance() >= animationController.GetArmLength())
			HoldStab();
		
		if (holdStab && stabHoldTimer < 0f)
			StopStab();
		
		swordController.DoUpdate();
		swordController.MoveSword(block, alignStab, stab, holdStab);
		
		stabHoldTimer -= Time.deltaTime;
    }
	
	public void Block() { block = true; }
	public void StopBlock() { block = false; }
	
	public void StartStab() { alignStab = true; }
	public void Stab() { alignStab = false; stab = true; }
	public void HoldStab() { stab = false; holdStab = true; stabHoldTimer = StabHoldDuration; }
	public void StopStab() { holdStab = false; }
	
	public void Collide(Collision collision) { physicsController.Collide(collision); }
	public void StopColliding() { physicsController.StopColliding(); }

	public Transform GetCamera() { return cam; }
	public Rigidbody GetRigidbody() { return physicsController.GetRigidbody(); }
	public Rigidbody GetSwordRigidbody() { return swordController.GetRigidbody(); }
	
	public Transform GetSwordModel() { return swordObject; }
	
	public Transform GetChest() { return animationController.GetChestBone(); }
	public Transform GetShoulder(bool right) { return animationController.GetShoulderBone(right); }
	public Transform GetArm(bool right) { return animationController.GetArmBone(right); }
	public Transform GetForeArm(bool right) { return animationController.GetForeArmBone(right); }
	public Transform GetHand(bool right) { return animationController.GetHandBone(right); }
	
	public Vector3 ArmToSword() { return swordController.GetRigidbody().position - animationController.GetArmBone(true).position; }
	public Vector3 ForeArmToSword() { return swordController.GetRigidbody().position - animationController.GetForeArmBone(true).position; }
	
	public float GetArmLength() { return animationController.GetArmLength(); }
	public float GetForeArmLength() { return animationController.GetForeArmLength(); }
	public float GetHoldDistance() { return animationController.GetArmLength() * (0.5f + (1f - swordController.GetArmBendAmount()) * 0.5f); }
	public float GetSwordDistance() { return Vector3.Distance(GetArm(true).position, swordController.GetRigidbody().position); }
	public float GetArmBendAmount() { return swordController.GetArmBendAmount(); }
	public float GetArmBendAngle() { return animationController.GetArmBendAngle(); }
	
	public bool GetBlock() { return block; }
	public bool GetAlignStab() { return alignStab; }
	public bool GetStab() { return stab; }
	public bool GetHoldStab() { return holdStab; }
}
