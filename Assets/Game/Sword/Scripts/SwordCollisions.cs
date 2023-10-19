using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisions
{
	public static SwordCollision SwordCollision(PlayerSwordController swordController, Collision col)
	{
		Vector3 colPoint = col.GetContact(0).point; // SwordCollisionPoint(swordController, col);
		
		return new SwordCollision(
			col.collider,
			colPoint,
			col.GetContact(0).normal, // SwordCollisionNormal(swordController, col, colPoint),
			col.relativeVelocity // swordController.physicsController.velocity
		);
	}
	
	public static Vector3 SwordCollisionPoint(PlayerSwordController swordController, Collider col)
	{
		return col.ClosestPointOnBounds(Vector3.Lerp(swordController.lastBasePosition, swordController.lastTipPosition, 0.5f));
	}
	
	public static Vector3 SwordCollisionNormal(PlayerSwordController swordController, Collider col, Vector3 collisionPoint)
	{
		Vector3 basePos = swordController.BasePosition();
		Vector3 tipPos = swordController.TipPosition();
		
		float lengthRatio = Vector3.Project(collisionPoint - basePos, swordController.Transform().forward).magnitude / swordController.length;
		
		Vector3 direction = swordController.physicsController.velocity.normalized;
		
		// float radius = 0.2f;
		
		// if (!Physics.SphereCast(collisionPoint - direction * (radius + 0.01f), radius, direction, out RaycastHit hit, radius + 0.02f))
		// {
			// Debug.Log("poopies");
			return -direction;
		// }
		
		// return hit.normal;
	}
}
