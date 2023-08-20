using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor.Animations;

public class PlayerAnimationController : MonoBehaviour
{
	[HideInInspector] public PlayerController playerController;
	[HideInInspector] public Animator animator;
	
	public AnimatorController animatorController;
	public Avatar animatorAvatar;
	
	public Transform rightArmIKTarget;
	private ArmIKTargetController swordArmIKTargetController;
	public Transform rightForeArmIKTarget;
	private ForeArmIKTargetController swordForeArmIKTargetController;
	
	public Transform hipBone;
	public Transform chestBone;
	public Transform rightShoulderBone;
	private Transform rightArmBone;
	private Transform rightForeArmBone;
	private Transform rightHandBone;

	private float humerusLength;
	private float foreArmLength;
	
	private float chestArmDistance;
	private float chestArmHandRatio;
	
	private float verticalArmDistance;
	private float horizontalArmDistance;
	
	public void DoUpdate() {
		swordArmIKTargetController.DoUpdate();
		swordForeArmIKTargetController.DoUpdate();
	}
	
    public void Initialize(PlayerController playerController)
    {	
		this.playerController = playerController;
		
		animator = GetComponent<Animator>();

		rightArmBone = rightShoulderBone.GetChild(0);
		rightForeArmBone = rightArmBone.GetChild(0);
		rightHandBone = rightForeArmBone.GetChild(0);
		
		humerusLength = Vector3.Distance(rightArmBone.position, rightForeArmBone.position);
		foreArmLength = Vector3.Distance(rightForeArmBone.position, rightHandBone.position);
		
		chestArmDistance = Vector3.Distance(chestBone.position, rightArmBone.position);
		chestArmHandRatio = (chestArmDistance / Vector3.Distance(rightArmBone.position, rightHandBone.position));
		
		verticalArmDistance = rightArmBone.position.y - transform.position.y;
		horizontalArmDistance = Vector3.Dot(rightArmBone.position - transform.position, transform.right);
	}
	
	public void InitializeSwordIKTargets(PlayerSwordController swordController)
	{
		Transform sword = swordController.transform.GetChild(0);
		
		// rightArmIKTarget = new GameObject($"{gameObject.name} SwordArm IK Target").transform;
		swordArmIKTargetController = rightArmIKTarget.gameObject.AddComponent<ArmIKTargetController>();
		swordArmIKTargetController.SetAnimationController(this);

		// rightForeArmIKTarget = new GameObject($"{gameObject.name} SwordForeArm IK Target").transform;
		swordForeArmIKTargetController = rightForeArmIKTarget.gameObject.AddComponent<ForeArmIKTargetController>();
		swordForeArmIKTargetController.SetSwordController(swordController);
	}
	
	public Vector3 SwordAimDirection()
	{
		return rightForeArmBone.up;
		
		// This is all IK stuff below

/*
		float armLength = GetArmLength();
		float d = playerController.GetSwordDistance();

		if (d >= armLength - 0.01f)
			return (playerController.GetSwordRigidbody().position - rightArmBone.position).normalized;
		
		float fA = HumerusAngle(d);
		
		Vector3 rotateFrom = playerController.GetSwordRigidbody().position - rightArmBone.position;
		Vector3 rotateTo = rotateFrom.normalized.y < 1f ? -Vector3.Cross(Vector3.up, rotateFrom).normalized : -Vector3.Cross(rotateFrom, Vector3.forward).normalized;
		
		rotateTo -= (Vector3.up - rotateTo) * Mathf.Min(playerController.ArmToSword().normalized.y, 0f) * -.25f;
		rotateTo.Normalize();
		
		Vector3 rotatedDir = Vector3.RotateTowards(rotateFrom, rotateTo, fA, 1f).normalized;
		
		return rotatedDir;
*/		
	}
	
	public bool GetArmIKControllerLocked() { return swordArmIKTargetController.GetLocked(); }
	
	public void LockArmIKTargetController() { swordArmIKTargetController.Lock(); }
	// public void UnlockArmIKTargetController() { swordArmIKTargetController.Unlock(); }
	
	public Transform GetRightArmIKTarget() { return rightArmIKTarget; }
	public Transform GetRightForeArmIKTarget() { return rightForeArmIKTarget; }
	
	public Transform GetChestBone() { return chestBone; }
	public Transform GetShoulderBone(bool right) { return right ? rightShoulderBone : null; }
	public Transform GetArmBone(bool right) { return right ? rightArmBone : null; }
	public Transform GetForeArmBone(bool right) { return right ? rightForeArmBone : null; }
	public Transform GetHandBone(bool right) { return right ? rightHandBone : null; }
	
	public Vector3 ApproximateChestPosition() {
		return new Vector3(
		transform.position.x, transform.position.y + verticalArmDistance, transform.position.z);
	}
	
	public Vector3 ApproximateArmPosition() {
		Vector3 chestToSwordApprox = playerController.ApproximateChestToSword();
		Vector3 camRightRotated = Vector3.Lerp(
		Vectors.FlattenVector(playerController.cam.right), chestToSwordApprox.normalized, chestArmHandRatio).normalized;
		
		return ApproximateChestPosition() + camRightRotated * chestArmDistance;
	}
	
	public Vector3 ArmRestPosition() {
		return ApproximateChestPosition() + playerController.cam.right * horizontalArmDistance;
	}

	public float GetHumerusLength() { return humerusLength; }
	public float GetForeArmLength() { return foreArmLength; }
	public float GetArmLength() { return humerusLength + foreArmLength; }
	public float GetArmBendAngle() { return Vector3.Angle(rightArmBone.up, rightForeArmBone.up); }

	public void SetFloat(string name, float setTo) { animator.SetFloat(name, setTo); }
	public void SetLayerWeight(int index, float setTo) { animator.SetLayerWeight(index, setTo); }
}
