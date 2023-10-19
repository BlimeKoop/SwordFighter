using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordMovementClamping
{
	public static Vector3 DistanceClampForce(PlayerSwordController swordController, PlayerController playerController,
	Vector3 clamping = new Vector3())
	{
		Vector3 armPositionApprox = playerController.animationController.ApproximateArmPosition();
		Vector3 pos = swordController.physicsController.NextPosition(clamping);
		Vector3 fromArm = pos - armPositionApprox;
		
		float minLength = playerController.animationController.ArmLength() / 5;
		float maxLength = playerController.animationController.ArmLength();
		
		if (fromArm.magnitude < minLength)
			return fromArm.normalized * (minLength - fromArm.magnitude);
		else if (fromArm.magnitude > maxLength)
			return fromArm.normalized * (maxLength - fromArm.magnitude);

		return Vector3.zero;
	}
	
	private static Vector3 VerticalArmClamping(PlayerController playerController, Vector3 clamping)
	{
		Vector3 swordPos = playerController.swordController.physicsController.Position();
		Vector3 fromArm = playerController.ArmToSword();
		Vector3 clampedPos = swordPos;
		
		float angleV = Vector3.Angle(fromArm, -Vector3.up);
		float minAngleV = 30f;
		
		if (angleV < minAngleV)
		{
			Vector3 clampedDir = Vector3.RotateTowards(fromArm, Vector3.up, (minAngleV - angleV) * Mathf.Deg2Rad, 1f).normalized;
			clampedPos = swordPos + clampedDir * fromArm.magnitude;
		}
		
		return (clampedPos - swordPos);
	}
	
	public static Vector3 HorizontalArmClampForce(PlayerSwordController swordController, PlayerController playerController)
	{
		if (playerController.block)
		{
			if (!playerController.SwordRight())
				return HorizontalArmClamping(playerController, swordController, 80f);
			else
				return HorizontalArmClamping(playerController, swordController, 120f);
		}
		
		if (!playerController.SwordRight())
			return HorizontalArmClamping(playerController, swordController, 45f);
		else
			return HorizontalArmClamping(playerController, swordController, 85f);
		
		/*
		float armfoldIncrease = 0.4f;
		float maxAngle = 80f;
		
		Vector3 armToSwordN = playerController.ArmToSword().normalized;
		
		// if (armToSwordN.y > 0.7f)
			// maxAngle *= 1.0f + armfoldIncrease * ((armToSwordN.y - 0.7f) / 0.3f);
		
		return ClampMovementHorizontally(
		playerController, swordController, force, maxAngle, !playerController.SwordRight());
		*/
	}
	
	private static Vector3 HorizontalArmClamping(PlayerController playerController, PlayerSwordController swordController, float maxForwardAngle)
	{ 
		Transform player = playerController.transform;

		Vector3 armPositionApprox = playerController.animationController.ApproximateArmPosition();
		Vector3 pos = swordController.physicsController.Position();
		Vector3 fromArmDir = (pos - armPositionApprox).normalized;
		
		Vector3 clampToward = Vectors.FlattenVector(playerController.camera.forward).normalized;

		float dot = Vector3.Dot(fromArmDir, clampToward);
		float dotMin = 1.0f - (maxForwardAngle / 90f);
		
		if (dot < dotMin)
		{
			return clampToward * (dotMin - dot);
		}

		return Vector3.zero;
	}
	
	public static Vector3 ForeArmClamping(PlayerController playerController, PlayerSwordController swordController,
	Vector3 clamping)
	{	
		Vector3 swordPos = swordController.physicsController.Position();
		
		Transform rightShoulder = playerController.animationController.bones["ShoulderBone"];
		Transform rightForeArm = playerController.animationController.bones["ForeArmBone"];
		
		Vector3 fromForeArm = swordPos - rightForeArm.position;
		Vector3 clampedPos = swordPos;
		
		float maxAngle = 100f;
		float angle = Vector3.Angle(fromForeArm, -rightShoulder.right);
		
		if (angle > maxAngle)
		{
			Vector3 clampedDir = Vector3.RotateTowards(fromForeArm, -rightShoulder.right, (angle - maxAngle) * Mathf.Deg2Rad, 1f).normalized;
			clampedPos = rightForeArm.position + clampedDir * fromForeArm.magnitude;
			
			// Debug.DrawRay(rightShoulder.position, -rightShoulder.right, Color.red, 1f);
		}

		return (clampedPos - swordPos);
	}
}
