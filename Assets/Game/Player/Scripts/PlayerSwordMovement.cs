using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordMovement
{
	public static Vector3 TipMovement(PlayerController playerController, Vector2 input)
	{
		Vector3 direction = TipDirection(playerController, input);

		float inputSpeed = Mathf.Min(input.magnitude, 150f);
		
		Vector3 movementR = direction * inputSpeed;
		movementR *= Mathf.Max(0.0f, 1.0f - playerController.swordController.weight);
		movementR *= playerController.swingSpeed;

		return movementR;
	}
	
	public static Vector3 BaseMovement(PlayerController playerController, Vector2 input)
	{
		Vector3 direction = BaseDirection(playerController, input);

		float inputSpeed = Mathf.Min(input.magnitude, 150f);
		
		Vector3 movementR = direction * inputSpeed;
		movementR *= Mathf.Max(0.0f, 1.0f - playerController.swordController.weight);
		movementR *= playerController.swingSpeed;
		// movementR = ClampMovement(playerController, movementR);
		movementR += DistanceMovement(playerController, playerController.swordController);

		return movementR;
	}
	
	public static Vector3 BlockMovement(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		/*
		Vector2 swingInputActive = inputController.GetSwingInputActive();
		
		Vector3[] orbitDirections = ShoulderOrbitDirections(playerController, swordController);

		Transform camera = playerController.camera;
		
		Vector3 horizontal = Vector3.Lerp(orbitDirections[0], Vectors.FlattenVector(camera.right), 0.4f);
		Vector3 vertical = Vector3.Lerp(orbitDirections[1], camera.up, 0.4f);
		
		Vector3 direction = (horizontal * swingInputActive.x + vertical * swingInputActive.y).normalized;
		
		float inputSpeed = Mathf.Min(inputController.GetSwingInput().magnitude, 150f);
		
		Vector3 movementR = direction * inputSpeed;
		movementR *= Mathf.Max(0.0f, 1.0f - swordController.weight);
		movementR *= playerController.swingSpeed;
		movementR += DistanceMovement(playerController, swordController) * 0.5f;
		
		return movementR;
		*/
		
		return new Vector3();
	}
	
	public static Vector3 AlignStabMovement(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		return new Vector3(); // SwingMovement(playerController, swordController, inputController);
	}
	
	public static Vector3 StabMovement(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
		return new Vector3(); // return SwingMovement(playerController, swordController, inputController);
	}
	
	public static Vector3 TipDirection(PlayerController playerController, Vector2 input)
	{
		PlayerSwordController swordController = playerController.swordController;
		
		/*
		if (swordController.swingLock)
			return swordController.physicsController.velocity.normalized;
		*/
		
		Vector3[] orbitDirections = OrbitDirections(swordController.GetBasePosition(), swordController.GetTipPosition());
		
		swordController.orbitDirectionsStore = orbitDirections;
		
		// Debug.DrawRay(swordController.GetTipPosition(), orbitDirections[0], Color.red);
		// Debug.DrawRay(swordController.GetTipPosition(), orbitDirections[1], Color.red);
		
		Vector3 directionR = (orbitDirections[0] * input.x + orbitDirections[1] * input.y).normalized;

		return directionR;
	}
	
	public static Vector3 BaseDirection(PlayerController playerController, Vector2 input)
	{
		PlayerSwordController swordController = playerController.swordController;
		
		/*
		if (swordController.swingLock)
			return swordController.physicsController.velocity.normalized;
		*/
		
		Vector3[] orbitDirections = OrbitDirections(
			swordController.playerController.animationController.ApproximateArmPosition(), swordController.GetBasePosition());
		
		swordController.orbitDirectionsStore = orbitDirections;
		
		Debug.DrawRay(swordController.GetBasePosition(), orbitDirections[0], Color.red);
		Debug.DrawRay(swordController.GetBasePosition(), orbitDirections[1], Color.red);
		
		Vector3 directionR = (orbitDirections[0] * input.x + orbitDirections[1] * input.y).normalized;
		
		// float t = 1f - (playerController.GetArmBendAngle() / 90f);
		// directionR = StraightenDirection(directionR, t, input, playerController);

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
	
	/*
	private static Vector3 ClampMovement(PlayerController playerController, Vector3 movement)
	{
		PlayerSwordController swordController = playerController.swordController;
		
		Vector3 fromTo = swordController.GetBasePosition() - playerController.animationController.ApproximateArmPosition();
		Vector3 fromToNext = (
			(swordController.GetBasePosition() + movement * Time.fixedDeltaTime) -
			playerController.animationController.ApproximateArmPosition());
			
		fromToNext = fromToNext.normalized * fromTo.magnitude;
		
		return (fromToNext - fromTo).normalized * movement.magnitude;
	}
	*/
	
	public static Vector3 DistanceMovement(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 swordPos = swordController.GetBasePosition();
		Vector3 movementR = new Vector3();

		float targetDistance = playerController.GetHoldDistance();
		
		Vector3 fromArm = playerController.ApproximateArmToSword();
		
		bool block = playerController.block;
		bool alignStab = playerController.alignStab;
		bool stab = playerController.stab;
		
		if (block)
		{
			Vector3 chestToSwordApprox = playerController.ApproximateChestToSword().normalized;
			float distance = Vector3.Dot(fromArm, chestToSwordApprox);
			
			if (!Mathf.Approximately(distance, targetDistance))
				movementR -= chestToSwordApprox * (distance - targetDistance);
		}
		else if (alignStab)
		{
			Vector3 chestToSwordApprox = playerController.ApproximateChestToSword().normalized;
			float distance = Vector3.Dot(fromArm, chestToSwordApprox);
			
			if (!Mathf.Approximately(distance, targetDistance))
				movementR -= chestToSwordApprox * (distance - targetDistance);
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
		
		return movementR;
	}
}
