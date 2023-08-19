using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
	public static Vector3 ArmIKTargetPosition(PlayerAnimationController animationController, PlayerController playerController)
	{
		Transform rightArmBone = animationController.GetArmBone(true);

		Vector3 approximateArmPos = playerController.animationController.ApproximateArmPosition();
		Vector3 armToSwordApprox = playerController.GetSwordRigidbody().position - approximateArmPos;

		float armLength = animationController.GetArmLength();
		float d = armToSwordApprox.magnitude;
		
		// if (d >= armLength - 0.01f)
			// return rightArmBone.position + (playerController.GetSwordRigidbody().position - rightArmBone.position).normalized * armLength;
		
		float hA = HumerusAngle(animationController, d);
		
		// Vector3 swingDir = playerController.inputController.SwingDirection().normalized;

		Vector3 rotateFrom = armToSwordApprox;
		Vector3 rotateTo = Vector3.Cross(Vector3.up, armToSwordApprox).normalized;
		
		rotateTo += (Vector3.up - rotateTo) * Mathf.Min(armToSwordApprox.normalized.y, 0f) * -.25f;
		rotateTo.Normalize();
		
		Vector3 rotatedDir = Vector3.RotateTowards(rotateFrom, rotateTo, hA, 1f).normalized;
		
		return approximateArmPos + rotatedDir * armLength / 2;
	}
	
	private static float HumerusAngle(PlayerAnimationController animationController, float distance)
	{
		float o = Mathf.Max(0f, animationController.GetArmLength() - distance);
		float h = animationController.GetHumerusLength();
		float r = Mathf.Clamp01(o / h);
		
		return Mathf.Asin(r);
	}
	
	private static float ForeArmAngle(PlayerAnimationController animationController, float distance)
	{
		float o = Mathf.Max(0f, animationController.GetArmLength() - distance);
		float h = animationController.GetForeArmLength();
		float r = Mathf.Clamp01(o / h);
		
		float angle = Mathf.Asin(r);
		
		return 90f - angle;
	}
}
