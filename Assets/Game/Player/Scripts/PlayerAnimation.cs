using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
	public static Vector3 ArmIKTargetPosition(PlayerAnimationController animationController, PlayerController playerController)
	{
		Vector3 approximateArmPos = playerController.animationController.ApproximateArmPosition();
		Vector3 armToSwordApprox = playerController.sword.position - approximateArmPos;

		float hA = Mathf.Max(0.0f, HumerusAngle(animationController, armToSwordApprox.magnitude));

		Vector3 rotateFrom = armToSwordApprox.normalized;
		Vector3 rotateTo = Vector3.Cross(Vector3.up, armToSwordApprox).normalized;
		
		Vector3 rotatedDir = Vector3.RotateTowards(rotateFrom, rotateTo, hA, 1f).normalized;
		
		return approximateArmPos + rotatedDir * animationController.ArmLength() / 2;
	}
	
	private static float HumerusAngle(PlayerAnimationController animationController, float distance)
	{
		float o = Mathf.Max(0f, animationController.ArmLength() - distance);
		float h = animationController.humerusLength;
		float r = Mathf.Clamp01(o / h);
		
		return Mathf.Asin(r);
	}
	
	private static float ForeArmAngle(PlayerAnimationController animationController, float distance)
	{
		float o = Mathf.Max(0f, animationController.ArmLength() - distance);
		float h = animationController.foreArmLength;
		float r = Mathf.Clamp01(o / h);
		
		float angle = Mathf.Asin(r);
		
		return 90f - angle;
	}
}
