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

		rigidbody.isKinematic = true;
	}
	
	public void MoveRigidbody(Vector3 movement)
	{
		Vector3 interpolatedMovement = Vector3.Lerp(rigidbody.velocity, movement, 0.1f);
		
		if (!collisionController.onGround)
			interpolatedMovement.y = Mathf.Max(Physics.gravity.y, rigidbody.velocity.y + Physics.gravity.y * Time.fixedDeltaTime);
		else
		{
			interpolatedMovement.y = Mathf.Clamp(collisionController.VerticalGroundOffset(), 0f, 0.2f) / Time.fixedDeltaTime;
		}
		
		if (swordCollisionController.colliding)
			interpolatedMovement = MoveAndSlide(interpolatedMovement, swordCollisionController.collision.point, swordCollisionController.collision.normal);
		if (collisionController.onWall)
			interpolatedMovement = MoveAndSlide(interpolatedMovement, collisionController.collisions["t"].GetContact(0).point, Collisions.InterpolatedNormal(collisionController.collisions["t"], 3));

		rigidbody.MovePosition(rigidbody.position + interpolatedMovement * Time.fixedDeltaTime);
	}
	
	private Vector3 MoveAndSlide(Vector3 movement, Vector3 collisionPoint, Vector3 collisionNormal, float friction = 0.0f)
	{
		Vector3 cross = Vectors.SafeCross(Vector3.up, collisionNormal).normalized;
		Vector3 movementR = movement + collisionNormal * Mathf.Max(0f, Vector3.Dot(movement, -collisionNormal));
		
		// Debug.DrawRay(collisionPoint, collisionNormal * 2f, Color.yellow);
		
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
		
		rigidbody.MoveRotation(newRotation);
	}
}
