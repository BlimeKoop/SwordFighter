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
	
	public Vector3 lastVelocity;
	
	[HideInInspector] public Vector3 baseForce;
	[HideInInspector] public Vector3 tipForce;
	
    public void Initialize(PlayerSwordController swordController)
    {
		this.swordController = swordController;
		collisionController = swordController.collisionController;
		playerController = swordController.playerController;
		
		rigidbody = PlayerSwordInitialization.Rigidbody(playerController);
		rigidbody = PlayerSwordInitialization.Rigidbody(playerController);
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

	public void Zero()
	{		
		rigidbody.velocity *= 0f;
		rigidbody.angularVelocity *= 0f;
		
		baseForce *= 0;
		tipForce *= 0;
	}

	public void Move(PlayerController playerController, PlayerSwordController swordController)
	{
		Vector3 dir = swordController.GetTipPosition() - swordController.GetBasePosition();
		Vector3 nextDir = (swordController.GetTipPosition() + tipForce * Time.fixedDeltaTime) - swordController.GetBasePosition();
		
		Vector3 axis = Vector3.Cross(dir, nextDir).normalized;
		
		// rigidbody.AddTorque(axis * tipForce.magnitude);
		
		rigidbody.AddForce(baseForce);
		rigidbody.AddForce(swordController.swordPlayerConstraint.positionOffset / Time.fixedDeltaTime * 1.07f);
		
		lastVelocity = rigidbody.velocity;
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
	}
	
	public void DampOutwardVelocity(PlayerSwordController swordController)
	{
		Vector3 fromTo = (
			swordController.GetBasePosition() -
			playerController.animationController.ApproximateArmPosition());
			
		rigidbody.AddForce(-fromTo.normalized * Vector3.Dot(rigidbody.velocity, fromTo.normalized));
	}
	
	public void RotateSword(Quaternion rotateTo)
	{
		Quaternion fromTo = rotateTo * Quaternion.Inverse(rigidbody.rotation);
		fromTo.ToAngleAxis(out float angle, out Vector3 axis);
		
		if (angle > 180f)
			angle = -(360f - angle);
		
		if (Mathf.Abs(angle) < 0.01f)
			return;

		rigidbody.AddTorque(axis.normalized * angle, ForceMode.VelocityChange);
	}
	
	public void CalculateBaseForce(Vector2 input)
	{
		baseForce = PlayerSwordMovement.BaseMovement(playerController, input) / Time.fixedDeltaTime;
		
		if (collisionController.Colliding())
		{
			baseForce = SwordPhysics.StickSwordMovementToCollision(this, baseForce);
		}
	}
		
	public void CalculateTipForce(Vector2 input)
	{
		tipForce = PlayerSwordMovement.TipMovement(playerController, input) / Time.fixedDeltaTime;
	}
	
	public Vector3 GetNextPosition() { return rigidbody.position + rigidbody.velocity * Time.fixedDeltaTime; }
	public Vector3 GetNextPosition(Vector3 movement)
	{
		return rigidbody.position + rigidbody.velocity * Time.fixedDeltaTime + movement;
	}
	public Vector3 RigidbodyPosition() { return rigidbody.position; }
	public Quaternion RigidbodyRotation() { return rigidbody.rotation; }
}
