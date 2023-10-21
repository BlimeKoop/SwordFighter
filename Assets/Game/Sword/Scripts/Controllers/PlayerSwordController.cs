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
	[HideInInspector] public SwordCutterBehaviour swordCutterBehaviour;
	
	[HideInInspector] public Transform rollController;
	
	[HideInInspector] public Quaternion stabRotation;
	
	[HideInInspector] public float weight = 0.8f;
	[HideInInspector] public float drag = 0.3f;
	[HideInInspector] public float length;
	[HideInInspector] public float grabPointRatio = 0.15f;
	[HideInInspector] public float armBendAmount = 0f;
	
	[HideInInspector] public float inputAngleChange, inputSpeedChange;
	
	private float hitCooldownTimer, straightenTimer;
	private float HitCooldown = 0.06f, StraightenDuration = 0.5f;
	
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
		swordCutterBehaviour = PlayerSwordInitialization.SwordCutterBehaviour(this);
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
		hitCooldownTimer -= Time.deltaTime;
	}
	
	private float ArmBendAmount()
	{
		if (playerController.block)
			return 0.7f;
		
		if (playerController.alignStab)
			return 0.6f;
		
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
	
	public bool TryCut(Collision col)
	{
		if (hitCooldownTimer > 0)
			return false;
		
		if (col.gameObject.layer == Collisions.SwordLayer)
			return false;
		
		Vector3 relativeVelocity = (
			col.rigidbody != null ?
			col.rigidbody.velocity - physicsController.positionDeltaTracker.positionDelta / Time.fixedDeltaTime :
			physicsController.positionDeltaTracker.positionDelta / Time.fixedDeltaTime);
		
		GameObject colObj = col.gameObject;
		
		if (colObj.GetComponentInParent<PlayerController>() == playerController)
			return false;
		
		if (colObj.name.Contains("Player"))
		{
			// if (relativeVelocity.magnitude < 1f)
				// return false;
		}
		else 
		{
			if (relativeVelocity.magnitude < 3f)
				return false;
			
			if (!BackOfObjectFound(col.GetContact(0).point, col.GetContact(0).normal))
				return false;
			
			if (Vector3.Scale(colObj.GetComponentInChildren<Renderer>().bounds.size,
			new Vector3(1f, 0f, 1f)).magnitude > 20f)
				return false;
		}

		Vector3 cross = Vector3.Cross(playerController.sword.transform.up, relativeVelocity);
		Vector3 cutAxis = Vector3.Cross(relativeVelocity, cross);

        PhotonView.RPC("CutObject", RpcTarget.AllBufferedViaServer, colObj.name, cutAxis, col.GetContact(0).point);

        hitCooldownTimer = HitCooldown;
		
		return true;
	}
	
	private void QueueObjectDestruction(GameObject obj)
	{
		GameObject destroy = obj;
		Transform parent = destroy.transform.parent;

		if (parent != null && parent.GetComponent<Rigidbody>() != null && parent.childCount < 2)
			destroy = parent.gameObject;

		if (destroy.GetComponent<PhotonView>() == null)
			RoomManager.photonView.RPC("DestroyObject", RpcTarget.AllBufferedViaServer, destroy.name);
		else
			RoomManager.photonView.RPC("NetworkDestroyObject", RpcTarget.AllViaServer, destroy.name);
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
		
		if (hitCooldownTimer > 0)
			return false;

		obj.GetComponent<Rigidbody>().AddForce(sword.forward * 2f, ForceMode.Impulse);

		Fracture f = obj.GetComponent<Fracture>();
		PlayerSwordInitialization.FractureComponent(f, this);

		f.CauseFracture();
		
		hitCooldownTimer = HitCooldown;
		
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