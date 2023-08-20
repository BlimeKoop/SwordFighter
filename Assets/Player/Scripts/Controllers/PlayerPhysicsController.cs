using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicsController : MonoBehaviour
{
	[HideInInspector] public PlayerController playerController;
	[HideInInspector] public PlayerInputController inputController;
	[HideInInspector] public PlayerAnimationController animationController;
	
	[HideInInspector] public Rigidbody rb;
	
	private bool colliding;
	private Collision collision;
    
    public void Initialize(
		PlayerController playerController, PlayerInputController inputController, PlayerAnimationController animationController)
    {
		this.playerController = playerController;
		this.inputController = inputController;
		this.animationController = animationController;
		
		rb = GetComponent<Rigidbody>() == null ? gameObject.AddComponent<Rigidbody>() : GetComponent<Rigidbody>();
		
        ConfigureRigidbody(rb);
    }

	private void ConfigureRigidbody(Rigidbody rb)
	{
		rb.isKinematic = true;
		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
		
		rb.interpolation = RigidbodyInterpolation.Interpolate;
	}
	
	public void MoveRigidbody(Vector3 movement)
	{
		Vector3 interpolatedMovement = Vector3.Lerp(rb.velocity, movement, 0.1f);
		
		if (colliding)
			interpolatedMovement += collision.contacts[0].normal * Mathf.Max(0f, Vector3.Dot(interpolatedMovement, -collision.contacts[0].normal));
		
		Vector3 newPosition = rb.position + Vector3.Scale(interpolatedMovement, new Vector3(1.0f, 0.0f, 1.0f)) * Time.fixedDeltaTime;
		
		rb.MovePosition(newPosition);	
	}
	
	public void RotateRigidbody()
	{
		Vector3 camForward = Camera.main.transform.forward;
		Vector3 camForwardFlat = Vector3.Scale(camForward, new Vector3(1f, 0f, 1f)).normalized;

		Quaternion targetRotation = Quaternion.LookRotation(camForwardFlat);
		targetRotation *= Quaternion.Euler(0f, 45f * inputController.GetMovementInput().x, 0f);
		
		Quaternion baseRotation = Quaternion.Lerp(rb.rotation, targetRotation, 0.13f);
		
		Vector3 toIK = animationController.GetRightArmIKTarget().position - rb.position;
		Vector3 toIKFlat = Vector3.Scale(toIK, new Vector3(1f, 0f, 1f)).normalized;
		
		Vector3 ikDirection = Vector3.Lerp(camForwardFlat, toIKFlat, 0.4f).normalized;
		Quaternion ikRotation = Quaternion.LookRotation(ikDirection);
		
		Quaternion newRotation = Quaternion.Lerp(baseRotation, ikRotation, 0.2f);
		
		rb.MoveRotation(newRotation);
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
}
