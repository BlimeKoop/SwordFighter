using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPhysicsController : MonoBehaviour
{
	PlayerSwordController swordController;
	
	[HideInInspector] public Rigidbody rb;

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

	public void MoveSword(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 playerMovement = playerController.physicsController.rb.velocity;
		Vector3 swordMovement = swordController.movement;
		
		/*
		float t = 0.12f;

		if (rb.velocity.sqrMagnitude < swordMovement.sqrMagnitude)
			t += 0.05f * (1.0f - Mathf.Clamp01(swordMovement.magnitude - rb.velocity.magnitude / 2f));
		
		var newMovement = Vector3.Lerp(rb.velocity, swordMovement, t); */
		var newMovement = new Vector3();
		
		newMovement += swordMovement;
		newMovement += playerMovement * 1.45f;
		
		// if (!colliding) {
			rb.velocity = newMovement;

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

/*
	public Vector3 GetNextUpdatePosition() { return transform.position + rb.velocity * Time.fixedDeltaTime; }
	public Vector3 GetNextUpdatePosition(Vector3 movement) {
		return transform.position + movement * Time.fixedDeltaTime;
	}
*/

	public Vector3 GetLastVelocity() { return lastVelocity; }
	public Vector3 GetLastAngularVelocity() { return lastAngularVelocity; }
	
	public Vector3 GetActiveVelocity() { return activeVelocity; }
}
