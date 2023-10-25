using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPhysics : MonoBehaviour
{
	public static Vector3 StickSwordMovementToCollision(SwordPhysicsController swordPhysicsController, Vector3 movement)
	{
		// This isn't called with vertical normals
		Vector3 colNormal = swordPhysicsController.collisionController.collision.normal;
		Vector3 movementR = movement + colNormal * Mathf.Max(0f, Vector3.Dot(movement, -colNormal));
		
		Vector3 horizontal = Vector3.Cross(Vector3.up, colNormal);
		Vector3 vertical = Vector3.Cross(horizontal, colNormal).normalized;
		
		movementR -= horizontal * Vector3.Dot(movementR, horizontal);
		movementR -= vertical * Vector3.Dot(movementR, vertical);
		
		Debug.DrawRay(swordPhysicsController.collisionController.collision.point, swordPhysicsController.collisionController.collision.normal,
		Color.cyan);
		
		return movementR;
	}
}
