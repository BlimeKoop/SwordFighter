using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class PlayerPhysicsController
{
	[HideInInspector] public PlayerController playerController;
	[HideInInspector] public PlayerInputController inputController;
	[HideInInspector] public PlayerAnimationController animationController;
	[HideInInspector] public SwordCollisionController swordCollisionController;
	
	[HideInInspector] public RigidbodySynchronizable rigidbodySync;
	
	private bool colliding;
	private Collision collision;

    public void Initialize(
		PlayerController playerController, PlayerInputController inputController, PlayerAnimationController animationController)
    {
		this.playerController = playerController;
		this.inputController = inputController;
		this.animationController = animationController;
		
		swordCollisionController = playerController.swordController.collisionController;
		
		rigidbodySync = (
			playerController.GetComponent<RigidbodySynchronizable>() == null ?
			playerController.gameObject.AddComponent<RigidbodySynchronizable>() :
			playerController.GetComponent<RigidbodySynchronizable>());

		playerController.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		
		collision = new Collision();
	}
	
	public void MoveRigidbody(Vector3 movement)
	{
		Vector3 interpolatedMovement = Vector3.Lerp(rigidbodySync.velocity, movement, 0.1f);
		
		if (playerController.groundHit.transform == null)
			interpolatedMovement.y = Mathf.Max(Physics.gravity.y, rigidbodySync.velocity.y + Physics.gravity.y * Time.fixedDeltaTime);
		else
			interpolatedMovement = Vector3.Scale(interpolatedMovement, new Vector3(1.0f, 0.0f, 1.0f));

		if (swordCollisionController.Colliding() &&
			swordCollisionController.collision.transform != playerController.transform)
		{
			interpolatedMovement += (
				swordCollisionController.collision.contacts[0].normal *
				Mathf.Max(0f, Vector3.Dot(interpolatedMovement, -swordCollisionController.collision.contacts[0].normal)));
		}

		rigidbodySync.velocity = interpolatedMovement;
	}
	
	public void RotateRigidbody()
	{
		Vector3 camForward = playerController.camera.transform.forward;
		Vector3 camForwardFlat = Vector3.Scale(camForward, new Vector3(1f, 0f, 1f)).normalized;

		Quaternion targetRotation = Quaternion.LookRotation(camForwardFlat);
		targetRotation *= Quaternion.Euler(0f, 45f * inputController.GetMovementInput().x, 0f);
		
		Quaternion baseRotation = Quaternion.Lerp(rigidbodySync.rotation, targetRotation, 0.13f);
		
		Vector3 toIK = animationController.swordArmIKTargetController.transform.position - rigidbodySync.position;
		Vector3 toIKFlat = Vector3.Scale(toIK, new Vector3(1f, 0f, 1f)).normalized;
		
		Vector3 ikDirection = Vector3.Lerp(camForwardFlat, toIKFlat, 0.4f).normalized;
		Quaternion ikRotation = Quaternion.LookRotation(ikDirection);
		
		Quaternion newRotation = Quaternion.Lerp(baseRotation, ikRotation, 0.2f);
		
		rigidbodySync.angularVelocity = Vector3.zero;
		rigidbodySync.MoveRotation(newRotation);
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
}
