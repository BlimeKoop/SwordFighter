using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPhysicsController : MonoBehaviour
{
	PlayerSwordController swordController;
	
	private Rigidbody rb;

	private bool colliding;
	private Collision collision;
	
	private Vector3 lastVelocity;
	private Vector3 lastAngularVelocity;
	
	private Vector3 activeVelocity;
	
	private Vector3 lastPosition;
	private Quaternion lastRotation;
	
    private void Awake()
    {
		swordController = GetComponent<PlayerSwordController>();
		
		rb = GetComponent<Rigidbody>();
    }
	
	private void FixedUpdate()
	{
		lastPosition = rb.position;
		lastRotation = rb.rotation;
		
		if (rb.velocity.magnitude > 0.1f)
			activeVelocity = rb.velocity;
	}

	public void FreezeRigidbodyUntilFixedUpdate()
	{
		rb.isKinematic = true;
		
		StartCoroutine(UnfreezeRigidbody());
	}
	
	IEnumerator UnfreezeRigidbody()
	{
		yield return new WaitForFixedUpdate();
		
		rb.isKinematic = false;
	}

	public void ZeroVelocity()
	{		
		rb.velocity *= 0f;
		rb.angularVelocity *= 0f;
	}
	
	public void ZeroStoredVelocity()
	{
		lastVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
	}

	public void RevertVelocity()
	{
		rb.velocity = lastVelocity;
		rb.angularVelocity = lastAngularVelocity;
	}
	
	public void RevertRigidbody()
	{
		rb.MovePosition(lastPosition);
		rb.MoveRotation(lastRotation);		
	}

	public void MoveSword(Vector3 movement, Vector3 playerMovement, Vector3 clamping)
	{
		float t = 0.12f;

		// if (rb.velocity.sqrMagnitude < movement.sqrMagnitude)
			// t += 0.05f * (1.0f - Mathf.Clamp01(movement.magnitude - rb.velocity.magnitude / 2f));
		
		Vector3 newMovement = movement; // Vector3.Lerp(rb.velocity, movement, t);
		
		// playerMovement -= clamping * Mathf.Min(Vector3.Dot(playerMovement, clamping), 0f);
		
		// if (!colliding) {
			rb.velocity = newMovement + playerMovement + clamping;

			return;
		// }
		
		/*

		ContactPoint contact = collision.contacts[0];
		
		movement += contact.normal * Mathf.Max(0f, Vector3.Dot(movement, -contact.normal));

		Transform nextTransform = Rigidbodies.ApproximateNextTransform(rb);
		
		Vector3 baseOffset = ((nextTransform.forward - transform.forward) *
		swordController.GetLength() * swordController.GetGrabPointRatio());
		
		Vector3 toHit = contact.point - rb.position;
		Vector3 toHitBO = baseOffset.normalized * Vector3.Dot(toHit, baseOffset);
		
		if (baseOffset.magnitude > toHitBO.magnitude)
			movement -= baseOffset.normalized * (baseOffset.magnitude - toHitBO.magnitude);
		
		Vector3 horizontal = Vector3.Cross(Vector3.up, contact.normal);
		Vector3 vertical = Vector3.Cross(horizontal, contact.normal).normalized;
		
		movement -= vertical * Vector3.Dot(movement, vertical);
		
		rb.velocity = movement + clamping;
		*/
	}
	
	public void RotateSword(Quaternion rotateTo)
	{
		rb.MoveRotation(rotateTo);
	}

	public void Collide(Collision collision)
	{
		this.collision = collision;

		colliding = true;
	}

	public void StopColliding()
	{
		colliding = false;
	}

	public Rigidbody GetRigidbody() { return rb; }
	
	public Vector3 GetNextPosition() { return rb.position + rb.velocity * Time.fixedDeltaTime; }
	public Vector3 GetNextPosition(Vector3 movement) {
		return rb.position + (rb.velocity + movement) * Time.fixedDeltaTime;
	}
	
	public Vector3 GetLastVelocity() { return lastVelocity; }
	public Vector3 GetLastAngularVelocity() { return lastAngularVelocity; }
	
	public Vector3 GetActiveVelocity() { return activeVelocity; }
}
