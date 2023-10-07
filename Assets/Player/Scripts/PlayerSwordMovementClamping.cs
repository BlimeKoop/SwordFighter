using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordMovementClamping
{
	public static Vector3 DistanceClampedMovement(PlayerController playerController, PlayerSwordController swordController,
	Vector3 movement)
	{
		Vector3 armPositionApprox = playerController.animationController.ApproximateArmPosition();
		Vector3 nextPosApprox = swordController.physicsController.GetNextPosition(movement);
		Vector3 fromArmNext = nextPosApprox - armPositionApprox;
		
		float minLength = playerController.animationController.GetArmLength() / 5;
		float maxLength = playerController.animationController.GetArmLength();
		
		if (fromArmNext.magnitude < minLength)
			return movement + fromArmNext.normalized * (minLength - fromArmNext.magnitude) / Time.fixedDeltaTime;
		else if (fromArmNext.magnitude > maxLength)
			return movement + fromArmNext.normalized * (maxLength - fromArmNext.magnitude) / Time.fixedDeltaTime;

		return movement;
	}
	
	public static Vector3 ArmClampedMovement(PlayerController playerController, PlayerSwordController swordController,
	Vector3 movement)
	{
		// Vector3 verticalClamping = VerticalArmClamping(playerController, movement);
		movement = HorizontalArmClamped(swordController, playerController, movement);

		return movement;
	}
	
	private static Vector3 VerticalArmClamping(PlayerController playerController, Vector3 clamping)
	{
		Vector3 swordPos = playerController.swordController.physicsController.RigidbodyPosition();
		Vector3 fromArm = playerController.ArmToSword();
		Vector3 clampedPos = swordPos;
		
		float angleV = Vector3.Angle(fromArm, -Vector3.up);
		float minAngleV = 30f;
		
		if (angleV < minAngleV)
		{
			Vector3 clampedDir = Vector3.RotateTowards(fromArm, Vector3.up, (minAngleV - angleV) * Mathf.Deg2Rad, 1f).normalized;
			clampedPos = swordPos + clampedDir * fromArm.magnitude;
		}
		
		return (clampedPos - swordPos) / Time.fixedDeltaTime;
	}
	
	private static Vector3 HorizontalArmClamped(PlayerSwordController swordController,
	PlayerController playerController, Vector3 movement)
	{
		if (playerController.block)
		{
			if (!playerController.SwordRight())
				return ClampMovementHorizontally(playerController, swordController, movement, 85f, true);
			else
				return ClampMovementHorizontally(playerController, swordController, movement, 40f, false);
		}
		
		if (!playerController.SwordRight())
			return ClampMovementHorizontally(playerController, swordController, movement, 45f, true);
		else
			return ClampMovementHorizontally(playerController, swordController, movement, 85f, true);
		
		/*
		float armfoldIncrease = 0.4f;
		float maxAngle = 80f;
		
		Vector3 armToSwordN = playerController.ArmToSword().normalized;
		
		// if (armToSwordN.y > 0.7f)
			// maxAngle *= 1.0f + armfoldIncrease * ((armToSwordN.y - 0.7f) / 0.3f);
		
		return ClampMovementHorizontally(
		playerController, swordController, movement, maxAngle, !playerController.SwordRight());
		*/
	}
	
	private static Vector3 ClampMovementHorizontally(PlayerController playerController, PlayerSwordController swordController, Vector3 movement, float maxForwardAngle, bool right)
	{ 
		Transform player = playerController.transform;

		Vector3 armPositionApprox = playerController.animationController.ApproximateArmPosition();
		Vector3 nextPosApprox = swordController.physicsController.GetNextPosition(movement);
		Vector3 fromArmNext = nextPosApprox - armPositionApprox;
		Vector3 fromArmNextN = fromArmNext.normalized;
		
		Vector3 clampTowards = Vectors.FlattenVector(playerController.camera.forward).normalized;

		float dot = Vector3.Dot(fromArmNextN, clampTowards);
		float dotMin = 1.0f - (maxForwardAngle / 90f);
		
		if (dot < dotMin)
		{
			Vector3 clampedDir = (fromArmNextN + clampTowards * (dotMin - dot)).normalized;
			Vector3 clamping = (clampedDir - fromArmNextN) * fromArmNext.magnitude / Time.fixedDeltaTime;
			
			return movement + clamping;
		}

		return movement;
	}
	
	public static Vector3 ForeArmClamping(PlayerController playerController, PlayerSwordController swordController,
	Vector3 clamping)
	{	
		Vector3 swordPos = swordController.GetComponent<Rigidbody>().position;
		
		Transform rightShoulder = playerController.animationController.rightShoulderBone;
		Transform rightForeArm = playerController.animationController.rightForeArmBone;
		
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

		return (clampedPos - swordPos) / Time.fixedDeltaTime;
	}
}
