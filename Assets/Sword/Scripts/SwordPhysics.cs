using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPhysics : MonoBehaviour
{
	public static Vector3 StickSwordMovementToCollision(SwordPhysicsController swordPhysicsController, Vector3 movement, Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		
		Vector3 movementR = movement + contact.normal * Mathf.Max(0f, Vector3.Dot(movement, -contact.normal));

		/*
		Transform nextTransform = Rigidbodies.ApproximateNextTransform(swordPhysicsController.rigidbody);

		Vector3 baseOffset = ((nextTransform.forward - swordPhysicsController.transform.forward) *
		swordPhysicsController.swordController.length * swordPhysicsController.swordController.GetGrabPointRatio());
		
		Vector3 toHit = contact.point - swordPhysicsController.rigidbody.position;
		Vector3 toHitBO = baseOffset.normalized * Vector3.Dot(toHit, baseOffset);
		
		if (baseOffset.magnitude > toHitBO.magnitude)
			movementR -= baseOffset.normalized * (baseOffset.magnitude - toHitBO.magnitude);
		*/
		
		Vector3 horizontal = Vector3.Cross(Vector3.up, contact.normal);
		Vector3 vertical = Vector3.Cross(horizontal, contact.normal).normalized;
		
		movementR -= vertical * Vector3.Dot(movementR, vertical);
		
		return movementR;
	}
}
