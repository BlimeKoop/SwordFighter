using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerPhysicsController
{
	[HideInInspector] public PlayerController playerController;
	private PlayerAnimationController animationController;
	private PlayerInputController inputController;
	private PlayerCollisionController collisionController;
	
	private SwordCollisionController swordCollisionController;
	
	[HideInInspector] public Rigidbody rigidbody;

    public void Initialize(
		PlayerController playerController)
    {
		this.playerController = playerController;
		this.animationController = playerController.animationController;
		this.inputController = playerController.inputController;
		this.collisionController = playerController.collisionController;
		
		swordCollisionController = playerController.swordController.collisionController;
		
		rigidbody = (
			playerController.GetComponentInChildren<Rigidbody>() == null ?
			playerController.transform.GetChild(0).gameObject.AddComponent<Rigidbody>() :
			playerController.GetComponentInChildren<Rigidbody>());
			
		// rigidbody.isKinematic = true;
	}
	
	public void MoveRigidbody(Vector3 movement)
	{
		Vector3 interpolatedMovement = Vector3.Lerp(rigidbody.velocity, movement, 0.1f);
		
		if (interpolatedMovement.sqrMagnitude < 0.01f)
			interpolatedMovement *= 0;
		
		if (!collisionController.onGround)
			interpolatedMovement.y = Mathf.Max(Physics.gravity.y, rigidbody.velocity.y + Physics.gravity.y * Time.fixedDeltaTime);
		else
		{
			interpolatedMovement.y = Mathf.Max(-20.0f, Mathf.Lerp(
				rigidbody.velocity.y,
				Mathf.Clamp(collisionController.VerticalGroundOffset(), 0f, 0.15f) / Time.fixedDeltaTime,
				0.4f));
		}
		
		if (swordCollisionController.colliding)
		{
			interpolatedMovement = MoveAndSlide(interpolatedMovement, swordCollisionController.collision);
		}
		
		if (collisionController.onWall)
		{
			interpolatedMovement = MoveAndSlide(interpolatedMovement, collisionController.collisions["w"]);
		}

		rigidbody.velocity = interpolatedMovement;
	}
	
	private Vector3 MoveAndSlide(Vector3 movement, SwordCollision collision, float friction = 0.0f)
	{
		return MoveAndSlide(movement, collision.point, collision.normal, friction);
	}
	
	private Vector3 MoveAndSlide(Vector3 movement, Collision collision, float friction = 0.0f)
	{
		return MoveAndSlide(movement, collision.GetContact(0).point, Collisions.InterpolatedNormal(collision), friction);
	}
	
	private Vector3 MoveAndSlide(Vector3 movement, Vector3 point, Vector3 normal, float friction)
	{
		Vector3 cross = Vectors.SafeCross(Vector3.up, normal).normalized;
		Vector3 movementR = movement + normal * Mathf.Max(0f, Vector3.Dot(movement, -normal));
		
		// Debug.DrawRay(collisionPoint, normal * 2f, Color.yellow);
		
		friction = Mathf.Clamp01(friction);
		
		if (friction == 0.0f)
			return movementR;
		
		float lr = Math.FloatN1P1(Vector3.Dot(movementR, cross));
		
		// movementR += cross * (movementR.magnitude - movement.magnitude) * lr * (1.0f - friction);
		
		return movementR;
	}
	
	public void Rotate()
	{
		Vector3 camForward = playerController.camera.transform.forward;
		Vector3 camForwardFlat = Vector3.Scale(camForward, new Vector3(1f, 0f, 1f)).normalized;

		Quaternion targetRotation = Quaternion.LookRotation(camForwardFlat);
		Quaternion baseRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, 0.13f);
		
		Vector3 toIK = animationController.swordArmIKTargetController.transform.position - rigidbody.position;
		Vector3 toIKFlat = Vector3.Scale(toIK, new Vector3(1f, 0f, 1f)).normalized;
		
		Vector3 ikDirection = Vector3.Lerp(camForwardFlat, toIKFlat, 0.4f).normalized;
		Quaternion ikRotation = Quaternion.LookRotation(ikDirection);
		
		Quaternion newRotation = Quaternion.Lerp(baseRotation, ikRotation, 0.2f);

		rigidbody.angularVelocity *= 0;
		rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, newRotation, 0.9f));
	}
	
	public void Jump()
	{
		float scaledJumpHeight = playerController.jumpHeight * 2.25f;
		
		if (rigidbody.velocity.y > 0)
			scaledJumpHeight += rigidbody.velocity.y ;
		
		rigidbody.velocity -= Vector3.up * rigidbody.velocity.y;
		rigidbody.AddForce(Vector3.up * scaledJumpHeight, ForceMode.VelocityChange);
	}
	
	// /*
	public void ZeroVelocity()
	{
		rigidbody.velocity *= 0;
		rigidbody.angularVelocity *= 0;
	}
	
	public void SetFrozen(bool setTo)
	{
		rigidbody.isKinematic = setTo;
	}
	// */
}
