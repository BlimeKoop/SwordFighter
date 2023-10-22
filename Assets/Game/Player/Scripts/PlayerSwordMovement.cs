using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordMovement
{
	public static Vector3 TipMovement(PlayerController playerController, Vector2 input)
	{
		Vector3 direction = TipDirection(playerController, input);

		float inputSpeed = Mathf.Min(input.magnitude, 0.2f);
		
		Vector3 movementR = direction * inputSpeed;
		movementR *= Mathf.Max(0.0f, 1.0f - playerController.swordController.weight);
		movementR *= playerController.swingSpeed;

		return movementR;
	}
	
	public static Vector3 SwingMovement(PlayerController playerController, Vector2 input)
	{		
		Vector3[] orbitDirections = OrbitDirections(
			playerController.animationController.ApproximateArmPosition(), playerController.swordController.HoldPosition());
		
		orbitDirections[1] = playerController.camera.up;
		
		Debug.DrawRay(playerController.swordController.HoldPosition(), orbitDirections[0], Color.grey);
		Debug.DrawRay(playerController.swordController.HoldPosition(), orbitDirections[1], Color.grey);
		
		Vector3 direction = (orbitDirections[0] * input.x + orbitDirections[1] * input.y).normalized;
		
		// float t = 1f - (playerController.ArmBendAngle() / 90f);
		// direction = StraightenDirection(direction, 0.5f, input, playerController);

		float inputSpeed = Mathf.Min(input.magnitude, 0.3f) * Time.fixedDeltaTime;
		
		Vector3 movementR = direction * inputSpeed;
		movementR *= Mathf.Max(0.0f, 1.0f - playerController.swordController.weight);
		movementR *= playerController.swingSpeed;

		return movementR;
	}
	
	public static Vector3 BlockMovement(PlayerController playerController, PlayerSwordController swordController, Vector2 input)
	{
		Transform camera = playerController.camera;
		
		Vector3[] orbitDirections = OrbitDirections(playerController.animationController.bones["c"].position, swordController.HoldPosition());

		Vector3 horizontal = orbitDirections[0]; // Vector3.Lerp(orbitDirections[0], Vectors.FlattenVector(camera.right), 0.4f);
		Vector3 vertical = camera.up;
		
		Debug.DrawRay(swordController.HoldPosition(), horizontal * 3f, Color.white);
		Debug.DrawRay(swordController.HoldPosition(), camera.up * 3f, Color.white);
		
		Vector3 direction = (horizontal * input.x + vertical * input.y).normalized;
		
		float inputSpeed = Mathf.Min(input.magnitude, 150f);
		
		Vector3 movementR = direction * inputSpeed;
		movementR *= Mathf.Max(0.0f, 1.0f - swordController.weight);
		movementR *= playerController.swingSpeed;
		
		return movementR;
	}
	
	public static Vector3 TipDirection(PlayerController playerController, Vector2 input)
	{
		PlayerSwordController swordController = playerController.swordController;
		
		/*
		if (swordController.swingLock)
			return swordController.physicsController.velocity.normalized;
		*/
		
		Vector3[] orbitDirections = OrbitDirections(swordController.HoldPosition(), swordController.TipPosition());
		
		swordController.orbitDirectionsStore = orbitDirections;
		
		// Debug.DrawRay(swordController.TipPosition(), orbitDirections[0], Color.grey);
		// Debug.DrawRay(swordController.TipPosition(), orbitDirections[1], Color.grey);
		
		Vector3 directionR = (orbitDirections[0] * input.x + orbitDirections[1] * input.y).normalized;

		return directionR;
	}
	
	private static Vector3 StraightenDirection(Vector3 direction, float straightenAmount, Vector2 swingInputActive, PlayerController playerController)
	{
		Transform camera = playerController.camera;
		
		Vector3 camSwingDir = Vectors.FlattenVector(camera.right) * swingInputActive.x + camera.up * swingInputActive.y;
		Vector3 cross = Vector3.Cross(camera.forward, camSwingDir);
		cross.Normalize();

		Vector3 directionR = direction - cross * Vector3.Dot(direction, cross) * straightenAmount;
		directionR.Normalize();
		
		return directionR;
	}
	
	private static Vector3[] OrbitDirections(Vector3 fromPos, Vector3 toPos)
	{
		Vector3 fromToDir = (toPos - fromPos).normalized;
		
		Vector3 c0 = Vector3.Cross(Vector3.up, fromToDir).normalized;
		Vector3 c1 = Vector3.Cross(fromToDir, c0).normalized;
		
		return new Vector3[2] { c0, c1 };
	}
	
	public static Vector3 DistanceMovement(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 movementR = new Vector3();

		float targetDistance = playerController.HoldDistance();
		
		bool block = playerController.block;
		bool alignStab = playerController.alignStab;
		bool stab = playerController.stab;
		
		Vector3 fromArm = playerController.ApproximateArmToSword();
		
		if (block || alignStab)
		{
			Vector3 chestToSwordApprox = playerController.ApproximateChestToSword();
			
			if (!Mathf.Approximately(chestToSwordApprox.magnitude, targetDistance))
				movementR += chestToSwordApprox.normalized * (targetDistance - chestToSwordApprox.magnitude);
		}
		else if (stab)
		{
			Vector3 forward = playerController.sword.forward;
			float forwardDistance = Vector3.Dot(fromArm, forward);
			
			if (forwardDistance < targetDistance)
				movementR += forward * (targetDistance - forwardDistance);
		}
		else
		{
			movementR += fromArm.normalized * (targetDistance - fromArm.magnitude);
		}
		
		return movementR / Time.fixedDeltaTime;
	}
}
