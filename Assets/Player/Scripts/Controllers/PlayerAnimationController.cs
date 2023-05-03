using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerAnimationController : MonoBehaviour
{
	private PlayerController playerController;
	
	public Transform rig;
	public Transform rightArmIKTarget;
	private ArmIKTargetController swordArmIKTargetController;
	public Transform rightForeArmIKTarget;
	private ForeArmIKTargetController swordForeArmIKTargetController;
	public Transform rightArmIKHelper;
	
	private Transform hipBone;
	private Transform chestBone;
	private Transform rightShoulderBone;
	private Transform rightArmBone;
	private Transform rightForeArmBone;
	private Transform rightHandBone;
	
	private float humerusLength;
	private float foreArmLength;
	
    public void Initialize(PlayerController playerController)
    {	
		this.playerController = playerController;
		
		hipBone = rig;
		chestBone = rig.GetChild(2).GetChild(0).GetChild(0);
		rightShoulderBone = chestBone.GetChild(2);
		rightArmBone = rightShoulderBone.GetChild(0);
		rightForeArmBone = rightArmBone.GetChild(0);
		rightHandBone = rightForeArmBone.GetChild(0);
		
		humerusLength = Vector3.Distance(rightArmBone.position, rightForeArmBone.position);
		foreArmLength = Vector3.Distance(rightForeArmBone.position, rightHandBone.position);
	}
	
	public void InitializeSwordIKTargets(PlayerSwordController swordController)
	{
		Transform sword = swordController.transform.GetChild(0);
		
		swordArmIKTargetController = rightArmIKTarget.gameObject.AddComponent<ArmIKTargetController>();
		swordArmIKTargetController.SetSword(sword);
		swordArmIKTargetController.SetAnimationController(this);

		swordForeArmIKTargetController = rightForeArmIKTarget.gameObject.AddComponent<ForeArmIKTargetController>();
		swordForeArmIKTargetController.SetSwordController(swordController);
	}
		
	public Vector3 ArmIKTargetPosition()
	{
		float armLength = GetArmLength();
		float d = playerController.GetSwordDistance();
		
		if (d >= armLength - 0.01f)
			return rightArmBone.position + (playerController.GetSwordRigidbody().position - rightArmBone.position).normalized * humerusLength;
		
		float hA = HumerusAngle(d);
		
		Vector3 rotateFrom = playerController.GetSwordRigidbody().position - rightArmBone.position;
		Vector3 rotateTo = rotateFrom.normalized.y < 1f ? Vector3.Cross(Vector3.up, rotateFrom).normalized : Vector3.Cross(rotateFrom, Vector3.forward).normalized;
		
		rotateTo += (Vector3.up - rotateTo) * Mathf.Min(playerController.ArmToSword().normalized.y, 0f) * -.25f;
		rotateTo.Normalize();
		
		Vector3 rotatedDir = Vector3.RotateTowards(rotateFrom, rotateTo, hA, 1f).normalized;
		
		return rightArmBone.position + rotatedDir * humerusLength;
	}
	
	public Vector3 SwordAimDirection()
	{
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
	}
	
	private float HumerusAngle(float distance)
	{
		float o = Mathf.Max(0f, GetArmLength() - distance);
		float h = humerusLength;
		float r = Mathf.Clamp01(o / h);
		
		return Mathf.Asin(r);
	}
	
	private float ForeArmAngle(float distance)
	{
		float o = Mathf.Max(0f, GetArmLength() - distance);
		float h = foreArmLength;
		float r = Mathf.Clamp01(o / h);
		
		float angle = Mathf.Asin(r);
		
		return 90f - angle;
	}
	
	public bool GetArmIKControllerLocked() { return swordArmIKTargetController.GetLocked(); }
	
	public void LockArmIKTargetController() { swordArmIKTargetController.Lock(); }
	// public void UnlockArmIKTargetController() { swordArmIKTargetController.Unlock(); }
	
	public Transform GetRightArmIKTarget() { return rightArmIKTarget; }
	public Transform GetRightForeArmIKTarget() { return rightForeArmIKTarget; }
	public Transform GetRightArmIKHelper() { return rightArmIKHelper; }
	
	public Transform GetChestBone() { return chestBone; }
	public Transform GetShoulderBone(bool right) { return right ? rightShoulderBone : null; }
	public Transform GetArmBone(bool right) { return right ? rightArmBone : null; }
	public Transform GetForeArmBone(bool right) { return right ? rightForeArmBone : null; }
	public Transform GetHandBone(bool right) { return right ? rightHandBone : null; }

	public float GetHumerusLength() { return humerusLength; }
	public float GetForeArmLength() { return foreArmLength; }
	public float GetArmLength() { return humerusLength + foreArmLength; }
	public float GetArmBendAngle() { return Vector3.Angle(rightArmBone.up, rightForeArmBone.up); }
}
