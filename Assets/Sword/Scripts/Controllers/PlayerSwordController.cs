using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicMeshCutter;

public class PlayerSwordController : MonoBehaviour
{
	[SerializeField]
	private Material cutMaterial;
	
	private SwordPlayerConstraint swordPlayerConstraint;
	private SwordPhysicsController physicsController;
	
	private PlayerController playerController;
	private PlayerAnimationController animationController;
	private PlayerInputController inputController;
	private SwordCollisionController collisionController;
	private SwordCutterBehaviour swordCutterBehaviour;
	
	private Transform rollController;
	private Rigidbody rb;
	
	private Quaternion rotation;
	
	private float VerticalY = 0.9f;
	
	private float weight = 0.8f;
	private float drag = 0.3f;

	private float length;
	private float grabPointRatio = 0.33f;
	
	private float armBendAmount = 0f;
	
	private float straightenTimer;
	private float straightenDuration = 0.5f;
	
	private float inputAngleChange = 0f;

	private Vector3 swingDirection;
	public Vector3 movement;
	public Vector3 clamping;
	
	private Vector3 playerPosStore;

	private Transform rotationProxy;
	
	private float hitCooldownTimer;
	private float HitCooldown = 0.012f;
	
	private int layerStore;
	
	private ArmIKHelperController ikHelperController;
	
	private bool backwardFlip;
	private bool heldBackwards;
	
	public void Initialize(
	PlayerController playerController, PlayerInputController inputController, PlayerAnimationController animationController)
	{	
		this.playerController = playerController;
		this.inputController = inputController;
		this.animationController = animationController;

		swordPlayerConstraint = gameObject.AddComponent<SwordPlayerConstraint>();
		swordPlayerConstraint.playerRB = playerController.GetRigidbody();

		rotationProxy = new GameObject(gameObject.name + "RotationProxy").transform;
		rollController = CreateRollController();

		rb = InitializeRigidbody();
		
		physicsController = gameObject.AddComponent<SwordPhysicsController>();

		rb.position = playerController.GetHand(true).position;

		animationController.InitializeSwordIKTargets(this);

		rollController.rotation = RollControllerRotation();

		physicsController.RotateSword(CalculateRotation());
		
		transform.position = rb.position;
		transform.rotation = rb.rotation;
		
		rotation = transform.rotation;
		
		collisionController = InitializeSwordCollisionController();
		swordCutterBehaviour = InitializeSwordCutterBehaviour();
		
		Transform sword = playerController.GetSwordModel();

		gameObject.layer = Collisions.swordLayer;
		sword.gameObject.layer = Collisions.swordLayer;

		Bounds meshBounds = sword.GetComponent<MeshFilter>().mesh.bounds;
		
		float sizeX = meshBounds.size.x;
		float sizeY = meshBounds.size.y;
		float sizeZ = meshBounds.size.z;

		float thickness = Mathf.Min(sizeX, Mathf.Min(sizeY, sizeZ));
		
		length = Mathf.Max(sizeX, Mathf.Max(sizeY, sizeZ));

		int longestDirection = 0;
		int shortestDirection = 0;
		
		if (length == sizeY)
			longestDirection = 1;
		else if (length == sizeZ)
			longestDirection = 2;
		
		if (thickness == sizeY)
			shortestDirection = 1;
		else if (thickness == sizeZ)
			shortestDirection = 2;

		sword.rotation = rb.rotation;
		
		if (longestDirection == 0)
			sword.rotation *= Quaternion.Euler(0f, -90f, 0f);
		else if (longestDirection == 1)
			sword.rotation *= Quaternion.Euler(-90f, 0f, 0f);

		Bounds rendBounds = sword.GetComponent<MeshRenderer>().bounds;
		Vector3 swordOriginToCenter = rendBounds.center - sword.position;

		sword.position = rb.position - swordOriginToCenter + transform.forward * length * (0.5f - grabPointRatio);
		sword.parent = rollController;
		
		StartCoroutine(CheckAngleChange());
	}

	private Transform CreateRollController()
	{
		Transform pc = new GameObject(gameObject.name + "RollController").transform;
		
		pc.rotation = transform.rotation;
		pc.position = transform.position;
		pc.parent = transform;
		
		return pc;
	}

	private Rigidbody InitializeRigidbody()
	{
		Rigidbody rigidbodyR = (
			GetComponent<Rigidbody>() == null ?
			gameObject.AddComponent<Rigidbody>() :
			GetComponent<Rigidbody>());
		
		rigidbodyR.useGravity = false;
		rigidbodyR.collisionDetectionMode = CollisionDetectionMode.Continuous;
		rigidbodyR.mass = 0.02f;
		rigidbodyR.drag = 15f;
		
		rigidbodyR.interpolation = RigidbodyInterpolation.Interpolate;
		
		return rigidbodyR;
	}
	
	private SpringJoint InitializeSpringJoint()
	{
		SpringJoint springJointR = (
			GetComponent<SpringJoint>() == null ?
			gameObject.AddComponent<SpringJoint>() :
			GetComponent<SpringJoint>());
		
		return springJointR;
	}

	private SwordCollisionController InitializeSwordCollisionController()
	{
		SwordCollisionController swordCollisionControllerR = GetComponent<SwordCollisionController>();

		if (swordCollisionControllerR == null)
			swordCollisionControllerR = gameObject.AddComponent<SwordCollisionController>();
		
		swordCollisionControllerR.SetPlayerController(playerController);
		
		return swordCollisionControllerR;
	}

	private SwordCutterBehaviour InitializeSwordCutterBehaviour()
	{
		SwordCutterBehaviour swordCutterBehaviourR = GetComponent<SwordCutterBehaviour>();

		if (swordCutterBehaviour == null)
			swordCutterBehaviourR = gameObject.AddComponent<SwordCutterBehaviour>();
		
		swordCutterBehaviourR.DefaultMaterial = cutMaterial;
		swordCutterBehaviourR.Separation = 0.1f;
		
		return swordCutterBehaviourR;
	}
	
	public void DoFixedUpdate()
	{
		Vector3 velocityStore = rb.velocity;		
		Vector3 playerMovement = swordPlayerConstraint.GetPlayerPositionOffset() * 1.425f / Time.fixedDeltaTime;

		physicsController.MoveSword(movement, playerMovement, clamping);

		UpdateRotation();

		rb.angularVelocity = Vector3.zero;
		
		movement = Vector3.zero;
		clamping = Vector3.zero;
	}
	
	public void UpdateRotation() {
		if (playerController.GetAlignStab() || playerController.GetStab())
			rotation = Quaternion.Lerp(transform.rotation, CalculateRotation(), 0.2f);
		else
			rotation = CalculateRotation();

		physicsController.RotateSword(rotation);
	}
	
	public void DoUpdate()
	{		
		if (!playerController.GetBlock() && !playerController.GetAlignStab() && !playerController.GetStab() && !playerController.GetHoldStab())
			rollController.rotation = RollControllerRotation();	
		
		if (inputController.GetSwingInput().magnitude > 0.3f)
			straightenTimer = straightenDuration;
		
		if (playerController.GetBlock())
			armBendAmount = 1f; // Mathf.Min(armBendAmount + Time.deltaTime * 3.5f, 1f);
		else if (playerController.GetAlignStab())
			armBendAmount = 1f; // Mathf.Min(armBendAmount + Time.deltaTime * 3.5f, 0.6f);
		else if (playerController.GetStab())
			armBendAmount = 0f; // Mathf.Max(0f, armBendAmount - Time.deltaTime * 5f);
		else
			armBendAmount = 0f; // Mathf.Max(0f, armBendAmount - Time.deltaTime * 4f);

		bool heldBackwardsStore = heldBackwards;
		
		heldBackwards = Vector3.Dot(playerController.ForeArmToSword(), Vectors.FlattenVector(playerController.GetCamera().forward)) < 0f;

		if (heldBackwards != heldBackwardsStore && playerController.ForeArmToSword().normalized.y > 0.7f)
		{
			backwardFlip = !backwardFlip;
			// Debug.Log("backwardFlip toggled");
		}
		
		if (backwardFlip)
			backwardFlip = MaintainGimbleLock();

		straightenTimer -= Time.deltaTime;
		hitCooldownTimer -= Time.deltaTime;
	}
	
	private bool MaintainGimbleLock()
	{
		Vector3 foreArmToSword = playerController.ForeArmToSword();
		
		if (foreArmToSword.normalized.y < -0.6f)
			return false;
		
		if (!heldBackwards && (inputAngleChange > 45f || inputController.GetSwingInput().magnitude < 0.01f))
			return false;

		return true;
	}

	public bool SwordHeldVertically()
	{
		Vector3 foreArmToSwordDir = playerController.ForeArmToSword().normalized;
		
		return foreArmToSwordDir.y >= VerticalY;
	}

	public void CalculateSwordMovement(bool block, bool alignStab, bool stab, bool holdStab)
	{
		if (block)
		{
			movement = PlayerSwordMovement.BlockMovement(playerController, this, inputController);
			return;
		}
		
		if (alignStab)
		{
			movement = PlayerSwordMovement.AlignStabMovement(playerController, this, inputController);
			return;
		}

		if (stab || holdStab)
		{
			movement = PlayerSwordMovement.StabMovement(playerController, this, inputController);
			return;
		}
		
		movement = PlayerSwordMovement.SwingMovement(playerController, this, inputController);
	}
	
	public void CalculateSwordClamping() {
		Vector3 newClamping = new Vector3();
		
		newClamping += PlayerSwordMovement.ArmClamping(playerController, this, newClamping);
		// newClamping += PlayerSwordMovement.ForeArmClamping(playerController, this, newClamping);
		// newClamping +=  PlayerSwordMovement.DistanceClamping(playerController, this, newClamping);
		
		clamping = newClamping;
	}

	private Quaternion CalculateRotation()
	{
		if (playerController.GetStab())
			return transform.rotation;
		
		Quaternion rotationR = Quaternion.identity;
		
		if (playerController.GetAlignStab())
		{
			Vector3 lookDir = Vector3.Lerp(
			Vectors.FlattenVector(playerController.animationController.chestBone.forward).normalized,
			playerController.ApproximateArmToSword().normalized,
			0.4f);

			return Quaternion.LookRotation(lookDir);
		}

		Transform cam = playerController.GetCamera();
		
		Vector3 up = SwordAimDirection();
		Vector3 upR = up.y < 1f ? Vector3.up : Vectors.FlattenVector(-cam.forward).normalized;

		rotationR = Quaternion.LookRotation(up, upR);
		
		if (playerController.GetBlock())
			return rotationR;
		
		float raiseFactor = 0; //Mathf.Max(0f, playerController.ArmToSword().normalized.y);
		float angle = -80f;
		
		Vector3 upP = Vectors.FlattenVector(up, cam.forward);
		Vector3 cross = Vector3.Cross(upP, cam.forward).normalized;
		
		rotationR = Quaternion.AngleAxis(angle * raiseFactor, cross) * rotationR;	

		if (playerController.GetHoldStab()) {
			float t = 1.0f - playerController.stabHoldTimer / playerController.StabHoldDuration;
			
			return Quaternion.Lerp(rb.rotation, rotationR, t);
		}

		return rotationR;
	}
	
	private Quaternion RollControllerRotation()
	{
		Vector3 rbVelocity = physicsController.GetRigidbody().velocity;
		
		if (rbVelocity.magnitude < 0.05f)
			return rollController.rotation;
		
		Vector3 direction = rbVelocity.normalized;

		float angle = Vector3.Angle(rollController.up, Vectors.FlattenVector(-direction, rollController.forward));
		float angleN1P1 = MathFunctions.FloatN1P1(Vector3.Dot(-rollController.right, -direction));
		
		Quaternion rotReturn = rollController.rotation * Quaternion.Euler(0f, 0f, angle * angleN1P1 * 0.2f);

		return rotReturn;
	}
	
	public bool TryCut(GameObject obj)
	{
		if (obj.GetComponent<Fracture>() != null)
			return false;
		
		if (physicsController.GetLastVelocity().magnitude < 8f)
			return false;
		
		if (hitCooldownTimer > 0)
			return false;

		swordCutterBehaviour.Cut(obj, gameObject, physicsController.GetLastVelocity().normalized);
		
		hitCooldownTimer = HitCooldown;
		
		return true;
	}
	
	public bool TryShatter(GameObject obj)
	{
		if (obj.GetComponent<Fracture>() == null)
			return false;
		
		if (physicsController.GetLastVelocity().magnitude < 8f)
			return false;
		
		if (hitCooldownTimer > 0)
			return false;

		obj.GetComponent<Rigidbody>().AddForce(transform.forward * 2f, ForceMode.Impulse);

		Fracture f = obj.GetComponent<Fracture>();
		InitializeFractureComponent(f);

		f.CauseFracture();
		
		hitCooldownTimer = HitCooldown;
		
		return true;
	}
	
	private void InitializeFractureComponent(Fracture f)
	{
		FractureOptions fo = f.fractureOptions;

		fo.useOrigin = true;
		fo.useAxis = true;
		
		fo.origin = transform.position;
		fo.axis = Vector3.Cross(rb.velocity, transform.forward).normalized;
		
		fo.asynchronous = true;
	}
	
	private IEnumerator CheckAngleChange()
	{
		Vector2 inputStore = inputController.GetSwingInput();
		
		yield return new WaitForSeconds(0.065f);
		
		if (inputController.GetSwingInput().magnitude < 0.8f || inputStore.magnitude < 0.8f)
		{
			inputAngleChange = 0f;
			
			StartCoroutine(CheckAngleChange());
			
			yield break;
		}

		inputAngleChange = Vector2.Angle(inputStore, inputController.GetSwingInput());
		
		StartCoroutine(CheckAngleChange());
	}
	
	
	public void BendSwordArm()
	{
		armBendAmount = 1.0f;
	}
	
	private Vector3 SwordAimDirection() { return animationController.SwordAimDirection(); }
	
	public Vector3 GetActiveVelocity() { return physicsController.GetActiveVelocity(); }
	
	public float GetInputAngleChange() { return inputAngleChange; }
	
	public void Collide(Collision collision) { physicsController.Collide(collision); }
	public void StopColliding() { physicsController.StopColliding(); }

	public float GetLength() { return length; }
	public float GetWeight() { return weight; }
	public float GetDrag() { return drag; }
	
	public float GetGrabPointRatio() { return grabPointRatio; }
	public float GetArmBendAmount() { return armBendAmount; }
	
	public bool GetGimbleLock() { return backwardFlip; }
	
	public SwordPhysicsController GetPhysicsController() { return physicsController; }
	public Rigidbody GetRigidbody() { return physicsController.GetRigidbody(); }
}
