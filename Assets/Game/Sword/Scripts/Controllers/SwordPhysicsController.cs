using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SwordPhysicsController
{
	[HideInInspector] public PlayerController playerController;
	[HideInInspector] public PlayerSwordController swordController;
	[HideInInspector] public SwordCollisionController collisionController;
	
	public Rigidbody rigidbody;
	
	[HideInInspector] public Vector3 baseForce;
	[HideInInspector] public Vector3 tipForce;
	
	[HideInInspector] public Vector3 velocity;
	[HideInInspector] public Vector3 lastVelocity;
	[HideInInspector] public Vector3 activeVelocity;
	
    public void Initialize(PlayerSwordController swordController)
    {
		this.swordController = swordController;
		collisionController = swordController.collisionController;
		playerController = swordController.playerController;
		
		rigidbody = PlayerSwordInitialization.Rigidbody(playerController);
		rigidbody = PlayerSwordInitialization.Rigidbody(playerController);
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

	public void Move(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 dir = swordController.GetTipPosition() - swordController.GetBasePosition();
		Vector3 nextDir = (swordController.GetTipPosition() + tipForce * Time.fixedDeltaTime) - swordController.GetBasePosition();
		Vector3 axis = Vector3.Cross(dir, nextDir).normalized;
		
		// Debug.Log(baseForce);
		
		// rigidbody.AddTorque(axis * tipForce.magnitude);
		rigidbody.AddForce(baseForce);
	}
	
	public void ClampPosition(PlayerSwordController swordController)
	{
		Vector3 fromTo = (
			swordController.GetBasePosition() -
			playerController.animationController.ApproximateArmPosition());
		
		if (fromTo.magnitude < playerController.animationController.GetArmLength())
			return;
		
		rigidbody.MovePosition(
			rigidbody.position -
			fromTo.normalized *
			(fromTo.magnitude - playerController.animationController.GetArmLength()));
			
		rigidbody.velocity -= fromTo.normalized * Vector3.Dot(rigidbody.velocity, fromTo.normalized);
	}
	
	private Vector3 InterpolateToMovement(Vector3 movement)
	{
		float distance = Vector3.Distance(velocity, movement);
		float distanceFactor = Mathf.Min(distance / 45f, 1.0f);
		float min_t = 0.15f, max_t = 0.7f;
		float t = 1; // Mathf.Lerp(min_t, max_t, 1.0f - distanceFactor);

		// if (rigidbody.velocity.sqrMagnitude < movement.sqrMagnitude)
			// t += 0.05f * (1.0f - Mathf.Clamp01(movement.magnitude - velocity.magnitude / 2f));
		
		return Vector3.Lerp(velocity, movement, t);
	}
	
	public void RotateSword(PlayerSwordController swordController)
	{
		// rigidbody.MoveRotation(Quaternion.LookRotation(swordController.GetTipPosition() - swordController.GetBasePosition()));
	}
	
	public void CalculateBaseForce(Vector2 input)
	{
		baseForce = PlayerSwordMovement.BaseMovement(playerController, input) / Time.fixedDeltaTime;
	}
		
	public void CalculateTipForce(Vector2 input)
	{
		tipForce = PlayerSwordMovement.TipMovement(playerController, input) / Time.fixedDeltaTime;
	}
	
	public Vector3 GetNextPosition() { return rigidbody.position + rigidbody.velocity * Time.fixedDeltaTime; }
	public Vector3 GetNextPosition(Vector3 movement)
	{
		return rigidbody.position + (rigidbody.velocity + movement) * Time.fixedDeltaTime;
	}
	public Vector3 RigidbodyPosition() { return rigidbody.position; }
	public Quaternion RigidbodyRotation() { return rigidbody.rotation; }
}
