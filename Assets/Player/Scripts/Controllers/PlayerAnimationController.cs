using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class PlayerAnimationController
{
	[HideInInspector] public PlayerController playerController;
	
	private RigidbodySynchronizable rigidbodySync;
	
	[HideInInspector] public Animator animator;
	[HideInInspector] public UnityEngine.Avatar animatorAvatar;
	
	[HideInInspector] public Transform rightArmIKTarget;
	[HideInInspector] public ArmIKTargetController swordArmIKTargetController;
	[HideInInspector] public Transform rightForeArmIKTarget;
	[HideInInspector] public ForeArmIKTargetController swordForeArmIKTargetController;
	
	[HideInInspector] public Transform hipBone;
	[HideInInspector] public Transform chestBone;
	[HideInInspector] public Transform rightShoulderBone;
	[HideInInspector] public Transform rightArmBone;
	[HideInInspector] public Transform rightForeArmBone;
	[HideInInspector] public Transform rightHandBone;
	
	private Transform transform;

	[HideInInspector] public float humerusLength;
	[HideInInspector] public float foreArmLength;
	
	[HideInInspector] public float chestArmDistance;
	[HideInInspector] public float chestArmHandRatio;
	
	[HideInInspector] public float verticalArmDistance;
	[HideInInspector] public float horizontalArmDistance;
	
	public void DoUpdate()
	{
		swordArmIKTargetController.DoUpdate();
		swordForeArmIKTargetController.DoUpdate();
		
		SetAnimatorFloat("Speed", rigidbodySync.velocity.magnitude * 0.35f);
		SetAnimatorLayerWeight(1, rigidbodySync.velocity.magnitude / playerController.moveSpeed);
	}
	
    public void Initialize(PlayerController playerController)
    {	
		this.playerController = playerController;
		
		rigidbodySync = playerController.GetComponent<RigidbodySynchronizable>();
		
		transform = playerController.transform;
		animator = playerController.GetComponent<Animator>();

		hipBone = playerController.rig;
		chestBone = hipBone.GetChild(2).GetChild(0).GetChild(0);
		rightShoulderBone = chestBone.GetChild(2);
		rightArmBone = rightShoulderBone.GetChild(0);
		rightForeArmBone = rightArmBone.GetChild(0);
		rightHandBone = rightForeArmBone.GetChild(0);
		
		humerusLength = Vector3.Distance(rightArmBone.position, rightForeArmBone.position);
		foreArmLength = Vector3.Distance(rightForeArmBone.position, rightHandBone.position);
		
		chestArmDistance = Vector3.Distance(chestBone.position, rightArmBone.position);
		chestArmHandRatio = (chestArmDistance * 1.3f / Vector3.Distance(rightArmBone.position, rightHandBone.position));
		verticalArmDistance = rightArmBone.position.y - transform.position.y;
		horizontalArmDistance = Vector3.Dot(rightArmBone.position - transform.position, transform.right);
		
		InitializeSwordIKTargets();
	}
	
	public void InitializeSwordIKTargets()
	{
		rightArmIKTarget = playerController.swordRig.Find("Right Arm IK Target");
		swordArmIKTargetController = rightArmIKTarget.gameObject.AddComponent<ArmIKTargetController>();
		swordArmIKTargetController.SetAnimationController(this);

		rightForeArmIKTarget = playerController.swordRig.Find("Right ForeArm IK Target");
		swordForeArmIKTargetController = rightForeArmIKTarget.gameObject.AddComponent<ForeArmIKTargetController>();
		swordForeArmIKTargetController.SetSwordController(playerController.swordController);
	}
	
	public Vector3 SwordAimDirection()
	{
		return (
			playerController.swordController.physicsController.RigidbodyPosition() -
			PlayerAnimation.ArmIKTargetPosition(this, playerController)).normalized;
	}
	
	public Vector3 ApproximateChestPosition() {
		return new Vector3(
		transform.position.x, transform.position.y + verticalArmDistance, transform.position.z);
	}
	
	public Vector3 ApproximateArmPosition() {
		Vector3 chestToSwordApprox = playerController.ApproximateChestToSword();
		Vector3 camRightRotated = Vector3.Lerp(
		Vectors.FlattenVector(playerController.camera.right), chestToSwordApprox.normalized, chestArmHandRatio).normalized;
		
		return ApproximateChestPosition() + Vectors.FlattenVector(playerController.camera.right) * chestArmDistance;
	}
	
	public Vector3 ArmRestPosition() {
		return ApproximateChestPosition() + playerController.camera.right * horizontalArmDistance;
	}
	public float GetArmLength() { return humerusLength + foreArmLength; }
	public float GetArmBendAngle() { return Vector3.Angle(rightArmBone.up, rightForeArmBone.up); }

	public void SetAnimatorFloat(string name, float setTo) { animator.SetFloat(name, setTo); }
	public void SetAnimatorLayerWeight(int index, float setTo) { animator.SetLayerWeight(index, setTo); }
}
