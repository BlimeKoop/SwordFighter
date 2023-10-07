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
		movementR *= Mathf.Max(0.0f, 1.0f - swordController.weight);
		movementR *= playerController.swingSpeed;
		movementR += DistanceMovement(playerController, swordController) * 0.5f;
		movementR /= Time.fixedDeltaTime;

		return movementR;
	}
	
	public static Vector3 BlockMovement(PlayerController playerController, PlayerSwordController swordController, PlayerInputController inputController)
	{
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
		if (swordController.swingLock)
			return swordController.physicsController.velocity.normalized;
		
		Vector2 swingInputActive = inputController.GetSwingInputActive();
				
		Vector3[] orbitDirections = ShoulderOrbitDirections(playerController, swordController);
		
		swordController.orbitDirectionsStore = orbitDirections;
		
		Debug.DrawRay(playerController.sword.position, orbitDirections[0], Color.blue);
		Debug.DrawRay(playerController.sword.position, orbitDirections[1], Color.green);
		
		Vector3 directionR = orbitDirections[0] * swingInputActive.x + orbitDirections[1] * swingInputActive.y;
		
		directionR.Normalize();
		
		float t = 1f - (playerController.GetArmBendAngle() / 90f);
		
		// directionR = StraightenDirection(directionR, t, swingInputActive, playerController);

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
	
	public static Vector3[] ShoulderOrbitDirections(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 armPos = playerController.animationController.rightArmBone.position;
		Vector3 swordPos = swordController.GetComponent<Rigidbody>().position;
		
		return OrbitDirections(playerController, swordController, armPos, swordPos);
	}
	
	public static Vector3[] ForeArmOrbitDirections(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 foreArmPos = playerController.animationController.rightForeArmBone.position;
		Vector3 swordPos = swordController.GetComponent<Rigidbody>().position;
		
		return OrbitDirections(playerController, swordController, foreArmPos, swordPos);
	}
	
	private static Vector3[] OrbitDirections(PlayerController playerController, PlayerSwordController swordController, Vector3 fromPos, Vector3 toPos)
	{
		Vector3 fromToDir = (toPos - fromPos).normalized;
		
		Vector3 c0 = Vector3.Cross(Vector3.up, fromToDir).normalized;
		Vector3 c1 = Vector3.Cross(fromToDir, c0).normalized;
		
		return new Vector3[2] { c0, c1 };
	}
	
	public static Vector3 DistanceMovement(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 swordPos = swordController.GetComponent<Rigidbody>().position;
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
}
