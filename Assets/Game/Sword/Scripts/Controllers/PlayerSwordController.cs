using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DynamicMeshCutter;
using static UnityEngine.GraphicsBuffer;

public class PlayerSwordController
{
	private PhotonView PhotonView;
	[HideInInspector] public Transform sword;
	
	[HideInInspector] public SwordPlayerConstraint swordPlayerConstraint;
	[HideInInspector] public SwordPhysicsController physicsController;
	
	[HideInInspector] public PlayerController playerController;
	[HideInInspector] public PlayerAnimationController animationController;
	[HideInInspector] public PlayerInputController inputController;
	[HideInInspector] public SwordCollisionController collisionController;
	
	[HideInInspector] public Transform rollController;
	
	[HideInInspector] public Quaternion stabRotation;
	
	[HideInInspector] public float weight = 0.8f;
	[HideInInspector] public float drag = 0.3f;
	[HideInInspector] public float length;
	[HideInInspector] public float grabPointRatio = 0.15f;
	[HideInInspector] public float armBendAmount = 0f;
	
	[HideInInspector] public float inputAngleChange, inputSpeedChange;
	
	private float straightenTimer;
	private float StraightenDuration = 0.5f;
	
	private Vector2 baseInput, tipInput;
	[HideInInspector] public Vector3 lastBasePosition, lastHoldPosition, lastTipPosition;
	
	private ArmIKHelperController ikHelperController;
	
	private bool heldBackwards;
	
	// Maintains the direction the sword is currently travelling in
	[HideInInspector] public bool swingLock, disableSwingLock;
	
	private float SwingLockMinSpeed = 0.03f, ArmDirYSwingLockMin = 0.94f;
	
	[HideInInspector]
	public Vector3[] orbitDirectionsStore;
	
	public void Initialize(
	PlayerController playerController, PlayerInputController inputController, PlayerAnimationController animationController)
	{
		this.playerController = playerController;
		this.inputController = inputController;
		this.animationController = animationController;

		sword = playerController.sword;
		rollController = sword.GetChild(0);
		
		PhotonView = sword.GetComponent<PhotonView>();
		
		collisionController = PlayerSwordInitialization.CollisionController(this);
		physicsController = PlayerSwordInitialization.PhysicsController(this);

		RecordPositions();

		swordPlayerConstraint = sword.gameObject.AddComponent<SwordPlayerConstraint>();
		swordPlayerConstraint.Initialize(this);
    }
    public void DoFixedUpdate()
    {
		if (!playerController.block &&
			!playerController.alignStab &&
			!playerController.stab &&
			!playerController.holdStab)
			rollController.rotation = RollControllerRotation();	
		
		swordPlayerConstraint.CalculateOffsets();
		
		physicsController.DampVelocity();
		physicsController.CalculateForces(baseInput);
		
		physicsController.Move(playerController, this);
		physicsController.Rotate(PlayerSword.CalculateRotation(this));
		
		if (collisionController.colliding)
			physicsController.HandleCollision();
		
		RecordPositions();
		swordPlayerConstraint.RecordOffsets();
		
		PlayerSwordMovementClamping.UpdateSide(playerController.SwordRight(), playerController.SwordFront());
	}
	
	private void RecordPositions()
	{
		lastBasePosition = BasePosition();
		lastHoldPosition = HoldPosition();
		lastTipPosition = TipPosition();
	}
	
	public void DoUpdate()
	{
		if (inputController.SwingInput().magnitude > 0.3f)
			straightenTimer = StraightenDuration;
		
		armBendAmount = ArmBendAmount();
		HandleSwingLock();

		straightenTimer -= Time.deltaTime;
	}
	
	private float ArmBendAmount()
	{
		if (playerController.block)
			return 0.4f;
		
		if (playerController.alignStab)
			return 0.41f;
		
		if (playerController.stab || playerController.holdStab)
			return 0f;
		
		if (armBendAmount < 1.0f)
		{
			return Mathf.Lerp(
			1.0f - playerController.SwordHoldDistance,
			0.0f,
			physicsController.baseForce.magnitude * Time.fixedDeltaTime / (playerController.swingSpeed / 2));
		}
		else
		{
			return Mathf.Min(armBendAmount + Time.deltaTime * 0.3f, 1.0f - playerController.SwordHoldDistance);
		}
	}
	
	private void HandleSwingLock()
	{
		if (swingLock)
		{			
			if (inputAngleChange > 60f ||
			inputController.SwingInput().magnitude < SwingLockMinSpeed ||
			!playerController.SwordHeldVertically(0f))
			{
				swingLock = false;
				disableSwingLock = true;
			}
		}
		else if (!disableSwingLock &&
			inputController.SwingInput().magnitude > SwingLockMinSpeed &&
			playerController.SwordHeldVertically(ArmDirYSwingLockMin))
		{
			swingLock = true;
		}
		
		if (disableSwingLock)
			disableSwingLock = playerController.SwordHeldVertically(ArmDirYSwingLockMin);
	}
	
	public void Disable()
	{
		physicsController.rigidbody.useGravity = true;
	}
	
	private Quaternion RollControllerRotation()
	{
		Vector3 rbVelocity = physicsController.velocity;
		
		if (rbVelocity.magnitude < 0.05f)
			return rollController.rotation;
		
		Vector3 direction = rbVelocity.normalized;

		float angle = Vector3.Angle(rollController.up, Vectors.FlattenVector(-direction, rollController.forward));
		float angleN1P1 = Math.FloatN1P1(Vector3.Dot(-rollController.right, -direction));
		
		Quaternion rotReturn = rollController.rotation * Quaternion.Euler(0f, 0f, angle * angleN1P1 * 0.2f);

		return rotReturn;
	}
	
	public bool TryCut(GameObject obj, Vector3 collisionPoint, Rigidbody rigidbody = null)
	{
		if (obj.tag == "CantCut")
		{
			Debug.Log($"{obj.name} tagged as CantCut");
			return false;
		}
		
		if (obj.layer == Collisions.SwordLayer)
			return false;

		if (obj.GetComponentInParent<PlayerController>() == playerController)
			return false;
		
		Vector3 relativeVelocity = (
			rigidbody != null ?
			rigidbody.velocity - physicsController.positionDeltaTracker.positionDelta / Time.fixedDeltaTime :
			physicsController.positionDeltaTracker.positionDelta / Time.fixedDeltaTime);
		
		if (obj.layer == Collisions.PlayerLayer)
		{
			// if (relativeVelocity.magnitude < 1f)
				// return false;
		}
		else 
		{
			// Debug.Log(relativeVelocity.magnitude);
			
			if (relativeVelocity.magnitude < playerController.moveSpeed + 2f)
				return false;
			
			// This is scuffed
			// if (!BackOfObjectFound(collisionPoint, -relativeVelocity))
				// return false;
			
			if (Vector3.Scale(Objects.GetComponentInFamily<Renderer>(obj).bounds.size,
			new Vector3(1f, 0f, 1f)).magnitude > 20f)
				return false;
		}

		Vector3 cutAxis = Vector3.Cross(relativeVelocity, playerController.sword.transform.forward);

        RoomManager.CutObject(obj, cutAxis, collisionPoint);
		
		return true;
	}
	
	private bool BackOfObjectFound(Vector3 collisionPoint, Vector3 collisionNormal)
	{
		float thickness = 0.9f;
		
		Vector3 origin = collisionPoint - collisionNormal * thickness;
		Vector3 direction = collisionNormal;
		float radius = 0.01f;

		if (!Physics.SphereCast(origin, radius, direction, out RaycastHit hit, thickness, ~(1 << Collisions.SwordLayer)))
		{
			Debug.DrawRay(origin - direction * radius, direction * (thickness - radius), Color.red, 1f);
			return false;
		}
		
		Debug.DrawRay(origin - direction * radius, direction * (thickness - radius), Color.green, 1f);
		return true;
	}
	
	public bool TryShatter(GameObject obj)
	{
		if (obj.GetComponent<Fracture>() == null)
			return false;
		
		if (physicsController.velocity.magnitude < 8f)
			return false;

		obj.GetComponent<Rigidbody>().AddForce(sword.forward * 2f, ForceMode.Impulse);

		Fracture f = obj.GetComponent<Fracture>();
		PlayerSwordInitialization.FractureComponent(f, this);

		f.CauseFracture();
		
		return true;
	}
	
	public void CheckInputChange(Vector2 inputStore)
	{
		if (inputStore.magnitude > 0.2f && inputController.SwingInput().magnitude < 0.1f)
			inputAngleChange = 0;
		else if (inputStore.magnitude > 0.01f)
			inputAngleChange = Vector2.Angle(inputStore, inputController.SwingInput());
		
		inputSpeedChange = Mathf.Abs(inputStore.magnitude - inputController.SwingInput().magnitude);
	}

	public Vector3 BasePosition() { return physicsController.Position() - sword.transform.forward * length * grabPointRatio; }
	public Vector3 HoldPosition() { return physicsController.Position(); }
	public Vector3 TipPosition() { return (BasePosition() + sword.transform.forward * length); }
	
	public Vector3 MiddlePoint() { return Vector3.Lerp(BasePosition(), TipPosition(), 0.5f); }
	
	public void SetBaseInput(Vector2 input)
	{
		baseInput = input;
	}
		
	public void SetTipInput(Vector2 input)
	{
		tipInput = input;
	}
	
	public void RecordStabRotation() { stabRotation = sword.rotation; }
	public void BendSwordArm() { armBendAmount = 1.0f; }
	public float GrabPointRatio() { return grabPointRatio; }
	
	public SwordPhysicsController PhysicsController() { return physicsController; }
	
	public Transform Transform() { return playerController.transform; }
}
