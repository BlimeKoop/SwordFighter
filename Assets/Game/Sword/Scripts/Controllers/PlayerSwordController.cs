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

	[HideInInspector] public Vector3 movement;
	private Quaternion rotation;
	
	private float hitCooldownTimer, straightenTimer;
	private float HitCooldown = 0.06f, StraightenDuration = 0.5f;
	
	private Vector2 baseInput, tipInput;
	
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
		rotation = sword.rotation;
		rollController = sword.GetChild(0);
		
		PhotonView = sword.GetComponent<PhotonView>();
		
		collisionController = PlayerSwordInitialization.CollisionController(this);
		swordCutterBehaviour = PlayerSwordInitialization.SwordCutterBehaviour(this);
		physicsController = PlayerSwordInitialization.PhysicsController(this);

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
		
		swordPlayerConstraint.SyncronizeProxy();
		swordPlayerConstraint.CalculateOffsets();
		
		physicsController.CalculateBaseForce(baseInput);
		physicsController.CalculateTipForce(tipInput);
		
		physicsController.ClampPosition(this);
		physicsController.DampOutwardVelocity(this);
		physicsController.Move(playerController, this);
		
		swordPlayerConstraint.RecordOffsets();
		
		UpdateRotation();
		physicsController.RotateSword(rotation);
		
		movement = Vector3.zero;
	}
	
	public void UpdateRotation() {
		if (playerController.alignStab || playerController.stab)
			rotation = Quaternion.Lerp(sword.rotation, PlayerSword.CalculateRotation(this), 0.2f);
		else
			rotation = PlayerSword.CalculateRotation(this);
	}
	
	public void DoUpdate()
	{
		if (inputController.GetSwingInput().magnitude > 0.3f)
			straightenTimer = StraightenDuration;
		
		armBendAmount = ArmBendAmount();
		HandleSwingLock();

		movement = PlayerSword.CalculateMovement(this);

		straightenTimer -= Time.deltaTime;
		hitCooldownTimer -= Time.deltaTime;
	}
	
	private float ArmBendAmount()
	{
		if (playerController.block)
			return 1f;
		
		if (playerController.alignStab)
			return 0.6f;
		
		if (playerController.stab || playerController.holdStab)
			return 0f;
		
		return 1.0f - playerController.SwordHoldDistance;
	}
	
	private void HandleSwingLock()
	{
		if (swingLock)
		{			
			if (inputAngleChange > 60f ||
			inputController.GetSwingInput().magnitude < SwingLockMinSpeed ||
			!playerController.SwordHeldVertically(0f))
			{
				swingLock = false;
				disableSwingLock = true;
			}
		}
		else if (!disableSwingLock &&
			inputController.GetSwingInput().magnitude > SwingLockMinSpeed &&
			playerController.SwordHeldVertically(ArmDirYSwingLockMin))
		{
			swingLock = true;
		}
		
		if (disableSwingLock)
			disableSwingLock = playerController.SwordHeldVertically(ArmDirYSwingLockMin);
	}
	
	private Quaternion RollControllerRotation()
	{
		Vector3 rbVelocity = physicsController.rigidbody.velocity;
		
		if (rbVelocity.magnitude < 0.05f)
			return rollController.rotation;
		
		Vector3 direction = rbVelocity.normalized;

		float angle = Vector3.Angle(rollController.up, Vectors.FlattenVector(-direction, rollController.forward));
		float angleN1P1 = MathFunctions.FloatN1P1(Vector3.Dot(-rollController.right, -direction));
		
		Quaternion rotReturn = rollController.rotation * Quaternion.Euler(0f, 0f, angle * angleN1P1 * 0.2f);

		return rotReturn;
	}
	
	public bool TryCut(Collision col)
	{
		GameObject obj = col.gameObject;
		
		if (hitCooldownTimer > 0)
			return false;
		
		if (obj == playerController.gameObject)
			return false;
		
		if (obj.GetComponent<Fracture>() != null)
			return false;
		
		if (!obj.name.Contains("Player") && col.relativeVelocity.magnitude < 10f ||
			col.relativeVelocity.magnitude < 2f)
			return false;
		
		/*if (!obj.name.Contains("Player") && !BackOfObjectFound(col))
			return false; */
		
		if (Vector3.Scale(obj.GetComponentInChildren<Renderer>().bounds.size,
			new Vector3(1f, 0f, 1f)).magnitude > 20f)
			return false;

        if (obj.GetComponent<Renderer>() == null)
            obj = obj.GetComponentInChildren<Renderer>().gameObject;

        Network.InstantiateCutParents(obj);

        PhotonView.RPC("CutObject", RpcTarget.All, obj.name, col.relativeVelocity, col.GetContact(0).point);

        hitCooldownTimer = HitCooldown;
		
		return true;
	}
	
	private bool BackOfObjectFound(Collision col)
	{
		float thickness = 0.9f;
		
		Vector3 origin = col.contacts[0].point - col.contacts[0].normal * thickness;
		Vector3 direction = col.contacts[0].normal;
		float radius = 0.01f;
		
		if (!Physics.SphereCast(origin, radius, direction, out RaycastHit hit, thickness * 0.9f, ~(1 << 6)))
		{
			Debug.DrawRay(origin - direction * radius, direction * (thickness * 0.9f - radius), Color.red, 3f);
			return false;
		}
		
		Debug.DrawRay(origin - direction * radius, direction * (thickness * 0.9f - radius), Color.green, 3f);
		return true;
	}
	
	public bool TryShatter(GameObject obj)
	{
		if (obj.GetComponent<Fracture>() == null)
			return false;
		
		if (physicsController.lastVelocity.magnitude < 8f)
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
		if (inputStore.magnitude > 0.2f && inputController.GetSwingInput().magnitude < 0.1f)
			inputAngleChange = 0;
		else if (inputStore.magnitude > 0.01f)
			inputAngleChange = Vector2.Angle(inputStore, inputController.GetSwingInput());
		
		inputSpeedChange = Mathf.Abs(inputStore.magnitude - inputController.GetSwingInput().magnitude);
	}
	
	public void Disable()
	{
		
	}
	
	public Vector3 GetBasePosition() { return physicsController.rigidbody.position; }
	public Vector3 GetTipPosition()
	{
		return (GetBasePosition() + sword.transform.forward * (length - length * grabPointRatio));
	}
	
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
	public float GetGrabPointRatio() { return grabPointRatio; }
	
	public SwordPhysicsController GetPhysicsController() { return physicsController; }
}
