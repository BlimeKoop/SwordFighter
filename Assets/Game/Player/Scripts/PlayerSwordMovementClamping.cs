using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordMovementClamping
{
	public static bool rightSide;
	
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
	
	public static Vector3 HorizontalArmClampForce(PlayerSwordController swordController, PlayerController playerController,
	float restorationMultiplier = 0.0f)
	{
		Vector3 clampingR;
		
		if (playerController.block)
		{
			if (!rightSide)
			{
				clampingR = ArmClamping(playerController, swordController, 80f,
				Vectors.FlattenVector(playerController.camera.forward), restorationMultiplier);
	
				return clampingR;
			}
			else
			{
				clampingR = ArmClamping(playerController, swordController, 100f,
				Vectors.FlattenVector(playerController.camera.forward), restorationMultiplier);
	
				return clampingR;
			}
		}
		
		if (!rightSide)
		{
			clampingR = ArmClamping(playerController, swordController, 45f,
			Vectors.FlattenVector(playerController.camera.forward), restorationMultiplier);

			return clampingR;
		}
		else
		{
			clampingR = ArmClamping(playerController, swordController, 140f,
			Vectors.FlattenVector(playerController.camera.forward), restorationMultiplier);
			
			if (!playerController.SwordFront())
				clampingR += ArmClamping(playerController, swordController, 96f,
				Vectors.FlattenVector(playerController.camera.right), restorationMultiplier);
				
			return clampingR;
		}
		
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
	
	private static Vector3 ArmClamping(PlayerController playerController, PlayerSwordController swordController,
	float maxForwardAngle, Vector3 clampToward, float restorationMultiplier = 0.0f, Vector3 currentClamping = new Vector3())
	{ 
		Transform player = playerController.transform;

		Vector3 armPositionApprox = playerController.animationController.ApproximateArmPosition();
		Vector3 pos = swordController.physicsController.Position() + currentClamping;
		Vector3 fromArm = (pos - armPositionApprox);
		
		clampToward.Normalize();

		float angle = Vector3.Angle(fromArm.normalized, clampToward);
		
		if (angle > maxForwardAngle)
		{
			Vector3 cross = (
				Mathf.Abs(fromArm.normalized.y) - Mathf.Abs(playerController.camera.up.y) != 0 ?
				Vector3.Cross(playerController.camera.up, fromArm) :
				Vector3.Cross(fromArm, playerController.camera.forward));
				
			Vector3 axis = (
				Vector3.Dot(fromArm, playerController.camera.up) > 0 ?
				Vector3.Cross(cross, fromArm).normalized :
				Vector3.Cross(fromArm, cross)).normalized;
			
			// Debug.DrawRay(pos, axis, Color.gray);
			
			// Could use trig instead for optimization ?
			Vector3 rotated = Vector3.RotateTowards(
				fromArm, clampToward, (angle - maxForwardAngle) * Mathf.Deg2Rad, 1.0f).normalized * fromArm.magnitude;
			Vector3 clampedPos = armPositionApprox + rotated * (1.0f + restorationMultiplier);
			Vector3 clamping = clampedPos - pos;
			Vector3 axisRestore = axis * (Vector3.Dot(fromArm, axis) - Vector3.Dot(rotated, axis)) * restorationMultiplier * 0.8f;
			
			return clamping + axisRestore;
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
	
	public static void UpdateSide(bool right, bool front)
	{
		if (front)
		{
			rightSide = right;
		}
	}
}
