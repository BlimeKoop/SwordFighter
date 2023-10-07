using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPhysicsController : MonoBehaviour
{
	[HideInInspector] public PlayerSwordController swordController;
	[HideInInspector] public SwordCollisionController collisionController;
	
	private Rigidbody rigidbody;
	
	[HideInInspector] public Vector3 velocity;
	[HideInInspector] public Vector3 lastVelocity;
	[HideInInspector] public Vector3 activeVelocity;
	
    public void Initialize(PlayerSwordController playerSwordController)
    {
		swordController = playerSwordController;
		collisionController = playerSwordController.collisionController;
		
		rigidbody = PlayerSwordControllerInitialization.InitializeRigidbody(this);
    }
	
	public void RecordTransformData()
	{		
		if (velocity.magnitude > 0.1f)
			activeVelocity = velocity;
	}

	public void FreezeRigidbodyUntilFixedUpdate()
	{
		rigidbody.isKinematic = true;
		
		StartCoroutine(UnfreezeRigidbody());
	}
	
	IEnumerator UnfreezeRigidbody()
	{
		yield return new WaitForFixedUpdate();
		
		rigidbody.isKinematic = false;
	}

	public void ZeroVelocity()
	{		
		rigidbody.velocity *= 0f;
		rigidbody.angularVelocity *= 0f;
	}

	public void MoveSword(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 swordMovement = swordController.movement;
		
		float distance = Vector3.Distance(velocity, swordMovement);
		float distanceFactor = Mathf.Min(distance / 45f, 1.0f);
		float min_t = 0.15f, max_t = 0.7f;
		float t = Mathf.Lerp(min_t, max_t, 1.0f - distanceFactor);

		// if (rigidbody.velocity.sqrMagnitude < swordMovement.sqrMagnitude)
			// t += 0.05f * (1.0f - Mathf.Clamp01(swordMovement.magnitude - velocity.magnitude / 2f));
		
		velocity = Vector3.Lerp(velocity, swordMovement, t);

		if (collisionController.Colliding() &&
			collisionController.collision.transform != playerController.transform)
			velocity = SwordPhysics.StickSwordMovementToCollision(
				this, swordMovement, collisionController.collision);

		lastVelocity = rigidbody.velocity;

		rigidbody.velocity = Vector3.zero;

		rigidbody.AddForce(velocity +
			swordController.swordPlayerConstraint.positionOffset / Time.fixedDeltaTime,
			ForceMode.VelocityChange);
	}
	
	public void RotateSword(Quaternion rotateTo)
	{
		Quaternion offset = rotateTo * Quaternion.Inverse(rigidbody.rotation);
		
		offset.ToAngleAxis(out float angle, out Vector3 axis);
		axis.Normalize();
		
		if (angle > 180f)
			angle = -(360f - angle);
		
		if (Mathf.Abs(angle) < 0.001f || axis.magnitude < 0.001f)
			return;
		
		rigidbody.angularVelocity = Vector3.zero;
		
		rigidbody.AddTorque(axis * angle, ForceMode.VelocityChange);
	}
	
	public Vector3 GetNextPosition() { return rigidbody.position + rigidbody.velocity * Time.fixedDeltaTime; }
	public Vector3 GetNextPosition(Vector3 movement) {
		return rigidbody.position + (rigidbody.velocity + movement) * Time.fixedDeltaTime;
	}

	public Vector3 RigidbodyPosition() { return rigidbody.position; }
	public Quaternion RigidbodyRotation() { return rigidbody.rotation; }
}
