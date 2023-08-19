using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordMovement
{
	public static Vector3 SwingMovement(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		Vector3 direction = SwingDirection(playerController, swordController, inputController);

		float inputSpeed = Mathf.Min(inputController.GetSwingInput().magnitude, 150f);
		
		Vector3 movementR = direction * inputSpeed;
		movementR *= Mathf.Max(0.0f, 1.0f - swordController.GetWeight());
		movementR *= playerController.swingSpeed;
		movementR += DistanceMovement(playerController, swordController) * 0.5f;
		movementR /= Time.fixedDeltaTime;

		return movementR;
	}
	
	public static Vector3 BlockMovement(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		Vector2 swingInputActive = inputController.GetSwingInputActive();
		
		Vector3[] orbitDirections = ShoulderOrbitDirections(playerController, swordController);

		Transform cam = playerController.GetCamera();
		
		Vector3 horizontal = Vector3.Lerp(orbitDirections[0], Vectors.FlattenVector(cam.right), 0.4f);
		Vector3 vertical = Vector3.Lerp(orbitDirections[1], cam.up, 0.4f);
		
		Vector3 direction = (horizontal * swingInputActive.x + vertical * swingInputActive.y).normalized;
		
		float inputSpeed = Mathf.Min(inputController.GetSwingInput().magnitude, 150f);
		
		Vector3 movementR = direction * inputSpeed;
		movementR *= Mathf.Max(0.0f, 1.0f - swordController.GetWeight());
		movementR *= playerController.swingSpeed;
		movementR += DistanceMovement(playerController, swordController) * 0.5f;
		movementR /= Time.fixedDeltaTime;
		
		return movementR;
	}
	
	public static Vector3 AlignStabMovement(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		return SwingMovement(playerController, swordController, inputController);
	}
	
	public static Vector3 StabMovement(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		return SwingMovement(playerController, swordController, inputController);
	}
	
	public static Vector3 SwingDirection(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		// if (swordController.SwordHeldVertically())
			// return swordController.GetRigidbody().velocity.normalized;
		
		Vector2 swingInputActive = inputController.GetSwingInputActive();
				
		Vector3[] orbitDirections = ShoulderOrbitDirections(playerController, swordController);
		
		Vector3 directionR = orbitDirections[0] * swingInputActive.x + orbitDirections[1] * swingInputActive.y;
		
		float t = 1f - (playerController.GetArmBendAngle() / 90f);
		
		directionR = StraightenDirection(directionR, t, swingInputActive, playerController);

		return directionR;
	}

	private static Vector3 StraightenDirection(Vector3 direction, float straightenAmount, Vector2 swingInputActive, PlayerController playerController)
	{
		Transform cam = playerController.GetCamera();
		
		Vector3 camSwingDir = Vectors.FlattenVector(cam.right) * swingInputActive.x + cam.up * swingInputActive.y;
		Vector3 cross = Vector3.Cross(cam.forward, camSwingDir);
		cross.Normalize();

		Vector3 directionR = direction - cross * Vector3.Dot(direction, cross) * straightenAmount;
		directionR.Normalize();
		
		return directionR;
	}
	
	public static Vector3[] ShoulderOrbitDirections(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 armPos = playerController.GetArm(true).position;
		Vector3 swordPos = swordController.GetRigidbody().position;
		
		return OrbitDirections(playerController, swordController, armPos, swordPos);
	}
	
	public static Vector3[] ForeArmOrbitDirections(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 foreArmPos = playerController.GetForeArm(true).position;
		Vector3 swordPos = swordController.GetRigidbody().position;
		
		// Debug.DrawLine(foreArmPos, swordPos, Color.blue);
		
		return OrbitDirections(playerController, swordController, foreArmPos, swordPos);
	}
	
	private static Vector3[] OrbitDirections(PlayerController playerController, PlayerSwordController swordController, Vector3 fromPos, Vector3 toPos)
	{
		Vector3 fromTo = toPos - fromPos;
		
		Vector3 c0 = fromTo.y < 1f ? Vector3.Cross(Vector3.up, fromTo).normalized : Vector3.Cross(fromTo, Vector3.forward).normalized;
		Vector3 c1 = Vector3.Cross(fromTo, c0).normalized;
		
		if (swordController.GetGimbleLock())
			c1 *= -1;

		return new Vector3[2] { c0, c1 };
	}
	
	public static Vector3 DistanceMovement(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 swordPos = swordController.GetRigidbody().position;
		Vector3 movementR = new Vector3();

		float targetDistance = playerController.GetHoldDistance();
		
		Vector3 fromArm = playerController.ApproximateArmToSword();
		
		bool block = playerController.GetBlock();
		bool alignStab = playerController.GetAlignStab();
		bool stab = playerController.GetStab();
		
		if (block)
		{
			Vector3 chestToSwordDirApprox = playerController.ApproximateChestToSword().normalized;
			float distance = Vector3.Dot(fromArm, chestToSwordDirApprox);
			
			if (!Mathf.Approximately(distance, targetDistance))
				movementR -= chestToSwordDirApprox * (distance - targetDistance);
		}
		else if (alignStab)
		{
			Vector3 chestToSwordDirApprox = playerController.ApproximateChestToSword().normalized;
			float distance = Vector3.Dot(fromArm, chestToSwordDirApprox);
			
			if (!Mathf.Approximately(distance, targetDistance))
				movementR -= chestToSwordDirApprox * (distance - targetDistance);
		}
		else if (stab)
		{
			Vector3 forward = swordController.transform.forward;
			float forwardDistance = Vector3.Dot(fromArm, forward);
			
			if (forwardDistance < targetDistance)
				movementR += forward * (targetDistance - forwardDistance);
		}
		else
		{
			movementR += fromArm.normalized * (targetDistance - fromArm.magnitude);
		}
		
		return movementR;
	}
	
	public static Vector3 DistanceClamping(PlayerController playerController, PlayerSwordController swordController,
	Vector3 clamping)
	{
		Vector3 swordPos = swordController.GetRigidbody().position;
		Vector3 clampedPos = swordPos;
		Vector3 armToSword = playerController.ArmToSword();
		
		float minLength = playerController.GetArmLength() / 5;
		float maxLength = playerController.GetArmLength();
		
		if (armToSword.magnitude < minLength)
			return armToSword.normalized * (minLength - armToSword.magnitude) / Time.fixedDeltaTime;
		else if (armToSword.magnitude > maxLength)
			return armToSword.normalized * (maxLength - armToSword.magnitude) / Time.fixedDeltaTime;

		return Vector3.zero;
	}
	
	public static Vector3 ArmClamping(PlayerController playerController, PlayerSwordController swordController,
	Vector3 clamping)
	{
		Vector3 verticalClamping = VerticalArmClamping(playerController, clamping);
		Vector3 horizontalClamping = HorizontalArmClamping(swordController, playerController, clamping);

		return horizontalClamping; // + (verticalClamping + horizontalClamping);
	}
	
	private static Vector3 VerticalArmClamping(PlayerController playerController, Vector3 clamping)
	{
		Vector3 swordPos = playerController.GetSwordRigidbody().position;
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
	
	private static Vector3 HorizontalArmClamping(PlayerSwordController swordController,
	PlayerController playerController, Vector3 clamping)
	{		
		if (!playerController.SwordRight())
			return FoldedHorizontalArmClamping(playerController, swordController, clamping);
		else if (!playerController.SwordFront())
			return BackwardHorizontalArmClamping(playerController, swordController, clamping);
	
		return new Vector3();
	}
	
	private static Vector3 FoldedHorizontalArmClamping(PlayerController playerController, PlayerSwordController swordController, Vector3 clamping)
	{ 
		Transform player = playerController.transform;

		Vector3 fromArmN = (
		swordController.GetPhysicsController().GetNextPosition() -
		playerController.animationController.ApproximateArmPosition()).normalized;

		Vector3 fromArmC = Vector3.Cross(Vector3.up, fromArmN).normalized;
		Vector3 rotateTo = (
			fromArmC * MathFunctions.FloatN1P1(Vector3.Dot(player.forward, fromArmC)));
		
		float angle = Vector3.Angle(Vectors.FlattenVector(fromArmN), Vectors.FlattenVector(player.forward));
		float angleMax = 20f;
		// float minBlockAngleH = 10f;
		
		if (angle > angleMax)
		{
			Vector3 clampedDir = Vector3.RotateTowards(
			fromArmN, rotateTo, (angle - angleMax) * Mathf.Deg2Rad, 1.0f).normalized;
			
			return ((clampedDir - fromArmN) *
			fromArmN.magnitude * swordController.movement.magnitude);
		} /*
		else if (playerController.GetBlock() && angleH < minBlockAngleH)
		{
			Vector3 clampedDir = Vector3.RotateTowards(fromArm, rotateTo, (minBlockAngleH - angleH) * Mathf.Deg2Rad, 1f).normalized;
			Vector3 clampedPos = armPos + clampedDir * fromArm.magnitude;
			
			return (clampedPos - swordPosN) / Time.fixedDeltaTime;
		} */

		return Vector3.zero;
	}

	private static Vector3 BackwardHorizontalArmClamping(PlayerController playerController, PlayerSwordController swordController, Vector3 clamping)
	{
		Transform player = playerController.transform;
		
		Vector3 fromArmN = (
		swordController.GetPhysicsController().GetNextPosition() -
		playerController.animationController.ApproximateArmPosition()).normalized;
		
		Vector3 fromArmC = Vector3.Cross(Vector3.up, fromArmN).normalized;
		Vector3 rotateTo = (
			fromArmC * MathFunctions.FloatN1P1(Vector3.Dot(player.forward, fromArmC)));
		
		float angle = Vector3.Angle(Vectors.FlattenVector(fromArmN), Vectors.FlattenVector(player.right));
		float angleMax = 8f;
		
		if (angle > angleMax)
		{
			Vector3 clampedDir = Vector3.RotateTowards(
			fromArmN, rotateTo, (angle - angleMax) * Mathf.Deg2Rad, 1.0f).normalized;
			
			return ((clampedDir - fromArmN) *
			fromArmN.magnitude * swordController.movement.magnitude);
		}
		
		return Vector3.zero;
	}
	
	public static Vector3 ForeArmClamping(PlayerController playerController, PlayerSwordController swordController,
	Vector3 clamping)
	{	
		Vector3 swordPos = swordController.GetRigidbody().position;
		
		Transform rightShoulder = playerController.GetShoulder(true);
		Transform rightForeArm = playerController.GetForeArm(true);
		
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