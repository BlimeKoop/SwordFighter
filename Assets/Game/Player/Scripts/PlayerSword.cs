using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword
{
	public static float OrientModelToLength(Transform sword, Transform swordModel)
	{
		MeshFilter meshFilter = swordModel.GetComponentInChildren<MeshFilter>();
		Bounds bounds = meshFilter.mesh.bounds;
		Vector3 size = Vector3.Scale(bounds.size, meshFilter.transform.lossyScale);
		
		float sizeX = Mathf.Abs(size.x);
		float sizeY = Mathf.Abs(size.y);
		float sizeZ = Mathf.Abs(size.z);

		float thickness = Mathf.Min(sizeX, Mathf.Min(sizeY, sizeZ));
		float length = Mathf.Max(sizeX, Mathf.Max(sizeY, sizeZ));

		int longestDirection = 0;
		// int shortestDirection = 0;
		
		if (length == sizeY)
			longestDirection = 1;
		else if (length == sizeZ)
			longestDirection = 2;
		
		/*
		if (thickness == sizeY)
			shortestDirection = 1;
		else if (thickness == sizeZ)
			shortestDirection = 2;
		*/
		
		swordModel.rotation = sword.rotation;
		
		if (longestDirection == 0)
			swordModel.rotation *= Quaternion.Euler(0f, -90f, 0f);
		else if (longestDirection == 1)
			swordModel.rotation *= Quaternion.Euler(-90f, 0f, 0f);
		
		return length;
	}
	
	public static Vector3 CalculateMovement(PlayerSwordController playerSwordController)
	{
		return new Vector3();
		/*
		PlayerController playerController = playerSwordController.playerController;
		PlayerInputController inputController = playerController.inputController;
		
		Vector3 movementR = new Vector3();
		
		if (playerController.block)
			movementR = PlayerSwordMovement.BlockMovement(playerController, playerSwordController, inputController);
		else if (playerController.alignStab)
			movementR = PlayerSwordMovement.AlignStabMovement(playerController, playerSwordController, inputController);
		else if (playerController.stab || playerController.holdStab)
			movementR = PlayerSwordMovement.StabMovement(playerController, playerSwordController, inputController);
		else
			movementR = PlayerSwordMovement.SwingMovement(playerController, playerSwordController, inputController);
		
		if (!playerController.stab)
			movementR = ClampMovement(playerSwordController, movementR);

		return movementR;
		*/
	}
	
	public static Quaternion CalculateRotation(PlayerSwordController playerSwordController)
	{
		PlayerController playerController = playerSwordController.playerController;
		
		Quaternion rotationR = Quaternion.identity;
		
		if (playerController.stab)
			return playerSwordController.physicsController.RigidbodyRotation();
		
		if (playerController.alignStab)
		{
			Vector3 lookDir = Vector3.Lerp(
			Vectors.FlattenVector(playerController.animationController.chestBone.forward).normalized,
			playerController.ApproximateArmToSword().normalized,
			0.5f);

			return Quaternion.LookRotation(lookDir);
		}

		Transform camera = playerController.camera;
		
		Vector3 forward = playerController.animationController.SwordAimDirection();
		Vector3 up = (
			forward.y < 1f ?
			Vector3.up :
			Vectors.FlattenVector(-camera.forward).normalized);

		rotationR = Quaternion.LookRotation(forward, up);
		
		if (playerController.block)
			return rotationR;
		
		if (playerController.holdStab)
		{
			float offset = playerController.GetStabHoldDuration() / 1.4f;
			float t = Mathf.Clamp01(
				(playerController.stabHoldTimer - offset) / (playerController.GetStabHoldDuration() - offset));
				
			rotationR = Quaternion.Lerp(playerSwordController.stabRotation, rotationR, t);
		}

		return rotationR;
	}
}
