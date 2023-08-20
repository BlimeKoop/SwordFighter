using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DynamicMeshCutter;

[RequireComponent(typeof(PlayerAnimationController))]

public class PlayerController : MonoBehaviour
{
	
	[HideInInspector] public PlayerInputController inputController;
	[HideInInspector] public PlayerAnimationController animationController;
	[HideInInspector] public PlayerPhysicsController physicsController;
	[HideInInspector] public PlayerCollisionController collisionController;
    [HideInInspector] public PlayerSwordController swordController;
	
	private PlayerInput input;
	
	private Camera _cam;
	public Transform cam;
	
	public Transform sword;
	[HideInInspector] public Transform swordObject;
	
	[SerializeField] private float movementSpeed = 7.0f;
	public float swingSpeed = 9.0f;

	[HideInInspector] public Vector3 movement; 
	
	[HideInInspector] public bool block, alignStab, stab, holdStab;
	
	[HideInInspector] public float stabHoldTimer;
	[HideInInspector] public float StabHoldDuration = 0.2f;

    void Awake()
    {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		
		inputController = PlayerControllerInitialization.InputController(this);

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
		
		swordObject = swordController.transform;
	}

	void FixedUpdate()
	{		
		physicsController.MoveRigidbody(movement);
		physicsController.RotateRigidbody();
	
		swordController.DoFixedUpdate();
	}

    void Update()
    {
		movement = inputController.SwingDirection() * movementSpeed;
		
		animationController.SetFloat("Speed", physicsController.GetRigidbody().velocity.magnitude * 0.35f);
		animationController.SetLayerWeight(1, physicsController.GetRigidbody().velocity.magnitude / movementSpeed);
		
		// if (!block && swordController.GetInputAngleChange() > 50f)
			// animationController.LockArmIKTargetController();
		
		if (stab && ArmToSword().magnitude >= animationController.GetArmLength())
			HoldStab();
		
		if (holdStab && stabHoldTimer < 0f)
			StopStab();

		swordController.DoUpdate();
		swordController.CalculateMovement(block, alignStab, stab, holdStab);
		
		animationController.DoUpdate();
		// swordController.UpdateRotation();
		
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
	
	public Transform GetSwordModel() { return sword; }
	public Transform GetSwordObject() { return swordObject; }
	
	public Transform GetChest() { return animationController.GetChestBone(); }
	public Transform GetShoulder(bool right) { return animationController.GetShoulderBone(right); }
	public Transform GetArm(bool right) { return animationController.GetArmBone(right); }
	public Transform GetForeArm(bool right) { return animationController.GetForeArmBone(right); }
	public Transform GetHand(bool right) { return animationController.GetHandBone(right); }
	
	public Vector3 ToSword() {
		return swordController.GetRigidbody().position - transform.position;
	}
	
	public Vector3 ArmToSword() {
		return swordController.GetRigidbody().position - animationController.GetArmBone(true).position;
	}

	public Vector3 ApproximateArmToSword() {
		return swordController.GetRigidbody().position - animationController.ApproximateArmPosition();
	}
	
	public Vector3 ForeArmToSword() {
		return swordController.GetRigidbody().position - animationController.GetForeArmBone(true).position;
	}
	
	public Vector3 ChestToSword() {
		return swordController.GetRigidbody().position - animationController.GetChestBone().position;
	}

	public Vector3 ApproximateChestToSword() {
		return swordController.GetRigidbody().position - animationController.ApproximateChestPosition();
	}
	
	public float GetArmLength() { return animationController.GetArmLength(); }
	public float GetForeArmLength() { return animationController.GetForeArmLength(); }
	public float GetHoldDistance() { return animationController.GetArmLength() * (0.5f + (1f - swordController.GetArmBendAmount()) * 0.5f); }

	public float GetArmBendAmount() { return swordController.GetArmBendAmount(); }
	public float GetArmBendAngle() { return animationController.GetArmBendAngle(); }
	
	public bool SwordFront() {
		return MathFunctions.FloatN1P1(Vector3.Dot(ArmToSword().normalized, cam.forward)) > 0;
	}
	public bool SwordRight() {
		return MathFunctions.FloatN1P1(Vector3.Dot(ArmToSword().normalized, cam.right)) > 0;
	}
	
	public bool GetBlock() { return block; }
	public bool GetAlignStab() { return alignStab; }
	public bool GetStab() { return stab; }
	public bool GetHoldStab() { return holdStab; }
}
