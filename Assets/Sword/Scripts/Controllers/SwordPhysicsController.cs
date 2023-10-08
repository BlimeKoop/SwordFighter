using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPhysicsController : MonoBehaviour
{
	[HideInInspector] public PlayerSwordController swordController;
	[HideInInspector] public SwordCollisionController collisionController;
	
	[HideInInspector] public Vector3 velocity;
	[HideInInspector] public Vector3 lastVelocity;
	[HideInInspector] public Vector3 activeVelocity;
	
    public void Initialize(PlayerSwordController playerSwordController)
    {
		swordController = playerSwordController;
		collisionController = playerSwordController.collisionController;
    }
	
	public void RecordTransformData()
	{		
		if (velocity.magnitude > 0.1f)
			activeVelocity = velocity;
	}

	/*
	public void FreezeRigidbodyUntilFixedUpdate()
	{
		rigidbody.isKinematic = true;
		
		StartCoroutine(UnfreezeRigidbody());
	}
	
	IEnumerator UnfreezeRigidbody()
	{
		yield return new WaitForFixedUpdate();
		
		rigidbody.isKinematic = false;
	} */

	public void ZeroVelocity()
	{		
		velocity *= 0f;
	}

	public void MoveSword(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 swordMovement = swordController.movement;
		
		velocity = InterpolateToMovement(swordMovement);
/*
		if (collisionController.Colliding() &&
			collisionController.collision.transform != playerController.transform)
			velocity = SwordPhysics.StickSwordMovementToCollision(
				this, swordMovement, collisionController.collision); */

		lastVelocity = velocity;

		transform.Translate(velocity, Space.World);
	}
	
	private Vector3 InterpolateToMovement(Vector3 movement)
	{
		float distance = Vector3.Distance(velocity, movement);
		float distanceFactor = Mathf.Min(distance / 45f, 1.0f);
		float min_t = 0.15f, max_t = 0.7f;
		float t = 1; // Mathf.Lerp(min_t, max_t, 1.0f - distanceFactor);

		// if (rigidbodySync.velocity.sqrMagnitude < movement.sqrMagnitude)
			// t += 0.05f * (1.0f - Mathf.Clamp01(movement.magnitude - velocity.magnitude / 2f));
		
		return Vector3.Lerp(velocity, movement, t);
	}
	
	public void RotateSword(Quaternion rotateTo)
	{
		/*
		Quaternion offset = rotateTo * Quaternion.Inverse(rigidbodySync.rotation);
		
		offset.ToAngleAxis(out float angle, out Vector3 axis);
		axis.Normalize();
		
		if (angle > 180f)
			angle = -(360f - angle);
		
		if (Mathf.Abs(angle) < 0.001f || axis.magnitude < 0.001f)
			return;
		
		rigidbodySync.angularVelocity = Vector3.zero;	
		rigidbodySync.AddTorque(axis * angle, ForceMode.VelocityChange);
		*/
		
		transform.rotation = rotateTo;
	}
	
	public Vector3 GetNextPosition() { return transform.position + velocity; } // { return rigidbodySync.position + rigidbodySync.velocity * Time.fixedDeltaTime; }
	public Vector3 GetNextPosition(Vector3 movement) { return transform.position + InterpolateToMovement(movement); } /*
		{
		return rigidbodySync.position + (rigidbodySync.velocity + movement) * Time.fixedDeltaTime;
	}
*/
	public Vector3 RigidbodyPosition() { return transform.position; } // rigidbodySync.position; }
	public Quaternion RigidbodyRotation() { return transform.rotation; } // rigidbodySync.rotation; }
}
