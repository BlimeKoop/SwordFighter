using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController
{
	[HideInInspector] public PlayerController playerController;
	
	private Rigidbody rigidbody;
	
	[HideInInspector] public Animator animator;
	[HideInInspector] public UnityEngine.Avatar animatorAvatar;
	
	[HideInInspector] public Transform RightArmIKTarget;
	[HideInInspector] public ArmIKTargetController swordArmIKTargetController;
	[HideInInspector] public Transform RightForeArmIKTarget;
	[HideInInspector] public ForeArmIKTargetController swordForeArmIKTargetController;
	
	public Dictionary<string, Transform> bones = new Dictionary<string, Transform>();
	
	private Transform transform;

	[HideInInspector] public float humerusLength;
	[HideInInspector] public float foreArmLength;
	
	[HideInInspector] public float chestArmDistance;
	[HideInInspector] public float chestArmHandRatio;
	
	[HideInInspector] public float verticalArmDistance;
	[HideInInspector] public float horizontalArmDistance;
	
	public void DoUpdate()
	{
		float horizontalSpeedAbs = rigidbody.velocity.magnitude - Mathf.Abs(rigidbody.velocity.y);
		float forwardSpeed = Mathf.Abs(Vector3.Dot(rigidbody.velocity, playerController.camera.forward));
		float speed = horizontalSpeedAbs * Math.FloatN1P1(forwardSpeed);
		
		if (playerController.crouching)
			speed *= 1.2f;
		
		SetAnimatorFloat("Speed", speed);
		SetAnimatorLayerWeight(1, rigidbody.velocity.magnitude / playerController.moveSpeed);
		SetAnimatorLayerWeight(2, Mathf.Lerp(GetAnimatorLayerWeight(2), playerController.crouching ? 0.76f : 0.0f, Time.deltaTime * 10f));
		
		if (!playerController.photonView.IsMine)
			return;
		
		swordArmIKTargetController.DoUpdate();
		swordForeArmIKTargetController.DoUpdate();
	}
	
    public void Initialize(PlayerController playerController)
    {	
		this.playerController = playerController;
		
		rigidbody = playerController.GetComponentInChildren<Rigidbody>();
		
		transform = playerController.transform;
		animator = playerController.GetComponentInChildren<Animator>();
		
		bones["r"] = playerController.rig;
		bones["s"] = bones["r"].GetChild(2);
		bones["c"] = bones["s"].GetChild(0).GetChild(0);
		bones["n"] = bones["c"].GetChild(1);
		bones["h"] = bones["n"].GetChild(0);
		bones["rs"] = bones["c"].GetChild(2);
		bones["ra"] = bones["rs"].GetChild(0);
		bones["rfa"] = bones["ra"].GetChild(0);
		bones["rh"] = bones["rfa"].GetChild(0);
		bones["lf"] = bones["r"].GetChild(0).GetChild(0).GetChild(0);
		bones["rf"] = bones["r"].GetChild(1).GetChild(0).GetChild(0);
		
		humerusLength = Vector3.Distance(bones["ra"].position, bones["rfa"].position);
		foreArmLength = Vector3.Distance(bones["rfa"].position, bones["rh"].position);
		
		chestArmDistance = Vector3.Distance(bones["c"].position, bones["ra"].position);
		chestArmHandRatio = (chestArmDistance * 1.3f / Vector3.Distance(bones["ra"].position, bones["rh"].position));
		verticalArmDistance = bones["ra"].position.y - transform.position.y;
		horizontalArmDistance = Vector3.Dot(bones["ra"].position - transform.position, transform.right);
		
		InitializeSwordIKTargets();
	}
	
	public void InitializeSwordIKTargets()
	{
		RightArmIKTarget = playerController.swordRig.Find("Right Arm IK Target");
		swordArmIKTargetController = RightArmIKTarget.gameObject.AddComponent<ArmIKTargetController>();
		swordArmIKTargetController.SetAnimationController(this);

		RightForeArmIKTarget = playerController.swordRig.Find("Right ForeArm IK Target");
		swordForeArmIKTargetController = RightForeArmIKTarget.gameObject.AddComponent<ForeArmIKTargetController>();
		swordForeArmIKTargetController.SetPlayerController(playerController);
	}
	
	public Vector3 SwordAimDirection()
	{
		return (
			playerController.swordController.physicsController.Position() -
			RightArmIKTarget.position);
	}
	
	public Vector3 ApproximateArmPosition() {
		Vector3 chestToSword = playerController.ChestToSword();
		Vector3 camRightRotated = Vector3.Lerp(
		Vectors.FlattenVector(playerController.camera.right), chestToSword.normalized, chestArmHandRatio).normalized;
		
		return bones["c"].position + Vectors.FlattenVector(playerController.camera.right) * chestArmDistance;
	}
	
	public Vector3 ChestToSword() {
		return playerController.swordController.physicsController.Position() - bones["c"].position;
	}
	
	public Vector3 ArmRestPosition() {
		return bones["c"].position + playerController.camera.right * horizontalArmDistance;
	}
	
	public float ArmLength() { return humerusLength + foreArmLength; }
	public float ArmBendAngle() { return Vector3.Angle(bones["ra"].up, bones["rfa"].up); }

	public float GetAnimatorFloat(string name) { return animator.GetFloat(name); }
	public void SetAnimatorFloat(string name, float setTo) { animator.SetFloat(name, setTo); }
	
	public float GetAnimatorLayerWeight(int index) { return animator.GetLayerWeight(index); }
	public void SetAnimatorLayerWeight(int index, float setTo) { animator.SetLayerWeight(index, setTo); }
}
