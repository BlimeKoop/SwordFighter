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
		movementR += DistanceMovement(playerController, swordController);
		movementR *= Mathf.Max(0.0f, 1.0f - swordController.GetWeight());
		movementR /= Time.fixedDeltaTime;

		return movementR;
	}
	
	public static Vector3 BlockMovement(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		Vector2 swingInputActive = inputController.GetSwingInputActive();
		
		Vector3[] orbitDirections = ShoulderOrbitDirections(playerController, swordController);

		Transform cam = playerController.GetCamera();
		
		Vector3 horizontal = Vector3.Lerp(orbitDirections[0], cam.right, 0.4f);
		Vector3 vertical = Vector3.Lerp(orbitDirections[1], cam.up, 0.4f);
		
		Vector3 direction = (horizontal * swingInputActive.x + vertical * swingInputActive.y).normalized;
		
		float inputSpeed = Mathf.Min(inputController.GetSwingInput().magnitude, 150f);
		
		Vector3 movementR = direction * inputSpeed;
		movementR += DistanceMovement(playerController, swordController);
		movementR *= Mathf.Max(0.0f, 1.0f - swordController.GetWeight());
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
/*
		Vector2 swingInputActive = inputController.GetSwingInputActive();
		
		Transform cam = playerController.GetCamera();
		
		Vector3 direction = (cam.right * swingInputActive.x + cam.forward * swingInputActive.y).normalized;
		
		float inputSpeed = Mathf.Min(inputController.GetSwingInput().magnitude, 150f);
		
		Vector3 movementR = direction * inputSpeed;
		movementR += DistanceMovement(playerController, swordController);
		movementR *= Mathf.Max(0.0f, 1.0f - swordController.GetWeight());
		movementR /= Time.fixedDeltaTime;
		
		return movementR;
*/		
	}
	
	public static Vector3 SwingDirection(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		// if (swordController.SwordHeldVertically())
			// return swordController.GetRigidbody().velocity.normalized;
		
		Vector2 swingInputActive = inputController.GetSwingInputActive();
				
		Vector3[] orbitDirections = ForeArmOrbitDirections(playerController, swordController);
		
		Vector3 directionR = orbitDirections[0] * swingInputActive.x + orbitDirections[1] * swingInputActive.y;
		
		float t = 1f - (playerController.GetArmBendAngle() / 90f);
		
		directionR = StraightenDirection(directionR, t, swingInputActive, playerController);

		return directionR;
	}

	private static Vector3 StraightenDirection(Vector3 direction, float straightenAmount, Vector2 swingInputActive, PlayerController playerController)
	{
		Transform cam = playerController.GetCamera();
		
		Vector3 camSwingDir = cam.right * swingInputActive.x + cam.up * swingInputActive.y;
		Vector3 cross = Vector3.Cross(cam.forward, camSwingDir);
		cross.Normalize();

		Vector3 directionR = direction - cross * Vector3.Dot(direction, cross) * straightenAmount;
		directionR.Normalize();
		
		return directionR;
	}
	
	public static Vector3[] ShoulderOrbitDirections(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 armPos = playerController.GetArm(true).position;
		Vector3 currentPosition = swordController.GetRigidbody().position;
		
		return OrbitDirections(playerController, swordController, armPos, currentPosition);
	}
	
	public static Vector3[] ForeArmOrbitDirections(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 foreArmPos = playerController.GetForeArm(true).position;
		Vector3 currentPosition = swordController.GetRigidbody().position;
		
		// Debug.DrawLine(foreArmPos, currentPosition, Color.blue);
		
		return OrbitDirections(playerController, swordController, foreArmPos, currentPosition);
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
		Vector3 currentPosition = swordController.GetRigidbody().position;
		Vector3 targetPosition = currentPosition;

		float targetDistance = playerController.GetHoldDistance();
		
		Vector3 armPos = playerController.GetArm(true).position;
		Vector3 fromArm = currentPosition - armPos;
		
		bool block = playerController.GetBlock();
		bool alignStab = playerController.GetAlignStab();
		bool stab = playerController.GetStab();
		
		if (block)
		{
			Vector3 forward = playerController.transform.forward;
			float forwardDistance = Vector3.Dot(fromArm, forward);
			
			if (forwardDistance > targetDistance)
				targetPosition -= forward * (forwardDistance - targetDistance);
		}
		else if (alignStab)
		{
			Vector3 fromArmF = Vectors.FlattenVector(fromArm);
			
			float distanceXZ = Mathf.Abs(fromArmF.x) + Mathf.Abs(fromArmF.z);
			
			if (distanceXZ != targetDistance)
				targetPosition += fromArmF.normalized * (targetDistance - distanceXZ);
		}
		else if (stab)
		{
			Vector3 forward = swordController.transform.forward;
			float forwardDistance = Vector3.Dot(fromArm, forward);
			
			if (forwardDistance < targetDistance)
				targetPosition += forward * (targetDistance - forwardDistance);
		}
		else
		{
			targetPosition += fromArm.normalized * (targetDistance - fromArm.magnitude);
		}
		
		return (targetPosition - currentPosition) * 6f;
	}
	public static Vector3 DistanceClamping(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 currentPosition = swordController.GetRigidbody().position;
		Vector3 clampedPosition = currentPosition;
		
		Vector3 pos = playerController.GetArm(true).position;
		Vector3 fromPos = currentPosition - pos;
		
		float maxLength = playerController.GetArmLength();
		
		if (fromPos.magnitude > maxLength)
			clampedPosition += fromPos.normalized * (maxLength - fromPos.magnitude);

		return (clampedPosition - currentPosition) / Time.fixedDeltaTime;
	}
	
	public static Vector3 ForeArmClamping(PlayerController playerController, PlayerSwordController swordController)
	{	
		Vector3 currentPosition = swordController.GetRigidbody().position;
		
		Transform rightShoulder = playerController.GetShoulder(true);
		Transform rightForeArm = playerController.GetForeArm(true);
		
		Vector3 fromForeArm = currentPosition - rightForeArm.position;
		Vector3 clampedPosition = currentPosition;
		
		float maxAngle = 100f;
		float angle = Vector3.Angle(fromForeArm, -rightShoulder.right);
		
		if (angle > maxAngle)
		{
			Vector3 clampedDir = Vector3.RotateTowards(fromForeArm, -rightShoulder.right, (angle - maxAngle) * Mathf.Deg2Rad, 1f).normalized;
			clampedPosition = rightForeArm.position + clampedDir * fromForeArm.magnitude;
			
			// Debug.DrawRay(rightShoulder.position, -rightShoulder.right, Color.red, 1f);
		}

		return (clampedPosition - currentPosition) / Time.fixedDeltaTime;
	}
}
