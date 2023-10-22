using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision
{
	public Collider collider;
	public GameObject gameObject;
	
	public Vector3 point;
	public Vector3 normal;
	public Vector3 relativeVelocity;
	
	public SwordCollision(
	Collider collider = null, Vector3 point = new Vector3(), Vector3 normal = new Vector3(), Vector3 relativeVelocity = new Vector3())
	{
		this.collider = collider;
		gameObject = collider.gameObject;
		
		this.point = point;
		this.normal = normal;
		this.relativeVelocity = relativeVelocity;
	}
}

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
		
		if (length == sizeY)
			longestDirection = 1;
		else if (length == sizeZ)
			longestDirection = 2;
		
		swordModel.rotation = sword.rotation;
		
		if (longestDirection == 0)
			swordModel.rotation *= Quaternion.Euler(0f, -90f, 0f);
		else if (longestDirection == 1)
			swordModel.rotation *= Quaternion.Euler(-90f, 0f, 0f);
		
		return length;
	}
	
	public static Vector3 CalculateMovement(PlayerSwordController playerSwordController, Vector2 input)
	{
		PlayerController playerController = playerSwordController.playerController;
		PlayerInputController inputController = playerController.inputController;
		
		Vector3 movementR = new Vector3();
		
		if (playerController.block)
			movementR = PlayerSwordMovement.BlockMovement(playerController, playerSwordController, input);
		else if (playerController.alignStab)
			movementR = PlayerSwordMovement.SwingMovement(playerController, input);
		else if (playerController.stab || playerController.holdStab)
			movementR = PlayerSwordMovement.SwingMovement(playerController, input);
		else
			movementR = PlayerSwordMovement.SwingMovement(playerController, input);

		return movementR;
	}
	
	public static Quaternion CalculateRotation(PlayerSwordController playerSwordController)
	{
		PlayerController playerController = playerSwordController.playerController;
		
		if (playerController.stab)
			return playerSwordController.physicsController.Rotation();
		
		if (playerController.alignStab)
		{
			Vector3 lookDir = Vector3.Lerp(
			Vectors.FlattenVector(playerController.animationController.bones["c"].forward).normalized,
			playerController.ApproximateArmToSword().normalized,
			0.5f);

			return Quaternion.LookRotation(lookDir);
		}

		Transform camera = playerController.camera;
		
		Vector3 forward = playerController.animationController.SwordAimDirection();
		
		if (playerController.block)
			forward = -Vector3.Cross(Vector3.up, playerController.animationController.ChestToSword()).normalized;
		
		Vector3 up = (
			forward.y < 1f ?
			Vector3.up :
			Vectors.FlattenVector(-camera.forward).normalized);

		Quaternion rotationR = Quaternion.LookRotation(forward, up);
		
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
