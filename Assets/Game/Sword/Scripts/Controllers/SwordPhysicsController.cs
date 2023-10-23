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
	[HideInInspector] public PositionDeltaTracker positionDeltaTracker;
	
	public Rigidbody rigidbody;
	
	public Vector3 velocity { get { return rigidbody.velocity; } }
	
	private Vector3 lastVelocity;
	private Vector3 lastAngularVelocity;
	
	[HideInInspector] public Vector3 baseForce;
	[HideInInspector] public Vector3 tipForce;
	[HideInInspector] public Vector3 distanceForce;
	[HideInInspector] public Vector3 swingClampingForce;
	[HideInInspector] public Vector3 distanceClampingForce;
	[HideInInspector] public Vector3 playerForce;
	
	[HideInInspector] public float linearDamping = 1.0f, angularDamping = 0.45f;
	[HideInInspector] public float drag = 0.7f;
	[HideInInspector] public float angularDrag = 0.75f;
	
	private float distanceForceMultiplier = 0.3f;
	
	private Vector3[] dampForces = new Vector3[3];
	
    public void Initialize(PlayerSwordController swordController)
    {
		this.swordController = swordController;
		collisionController = swordController.collisionController;
		playerController = swordController.playerController;
		positionDeltaTracker = playerController.sword.GetComponent<PositionDeltaTracker>();
		
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

	public void DampVelocity()
	{
		foreach(Vector3 f in dampForces)
		{
			if (f.sqrMagnitude == 0)
				continue;
			
			rigidbody.velocity -=
			f.normalized *
			Mathf.Clamp(Vector3.Dot(rigidbody.velocity, f.normalized), 0.0f, f.magnitude) * linearDamping;
		}

		if (playerForce.sqrMagnitude > 0)
		{
			rigidbody.velocity -=
			playerForce.normalized *
			Mathf.Clamp(Vector3.Dot(rigidbody.velocity, playerForce.normalized), 0.0f, playerForce.magnitude);
		}
		
		rigidbody.velocity *= 1.0f - drag;
		rigidbody.angularVelocity *= 1.0f - angularDrag;
	}
		
	public void CalculateForces(Vector2 input)
	{
		baseForce = PlayerSwordMovement.SwingMovement(playerController, input) / Time.fixedDeltaTime;
		
		distanceForce = PlayerSwordMovement.DistanceMovement(playerController, swordController);
		swingClampingForce = PlayerSwordMovementClamping.HorizontalArmClampForce(swordController, playerController);
		distanceClampingForce = PlayerSwordMovementClamping.DistanceClampForce(swordController, playerController);
		playerForce = swordController.swordPlayerConstraint.positionOffset / Time.fixedDeltaTime;
		
		baseForce += swingClampingForce.normalized * Mathf.Max(0.0f, Vector3.Dot(baseForce, -swingClampingForce.normalized));
		distanceForce += swingClampingForce.normalized * Mathf.Max(0.0f, Vector3.Dot(distanceForce, -swingClampingForce.normalized));
		rigidbody.velocity += swingClampingForce.normalized * Mathf.Max(0.0f, Vector3.Dot(rigidbody.velocity, -swingClampingForce.normalized));
		
		dampForces[0] = distanceForce;
		dampForces[1] = swingClampingForce;
		dampForces[2] = distanceClampingForce;
	}

	public void Move(PlayerController playerController, PlayerSwordController swordController)
	{
		rigidbody.AddForce(baseForce);
		rigidbody.AddForce(distanceForce * distanceForceMultiplier, ForceMode.VelocityChange);
		rigidbody.AddForce(playerForce, ForceMode.VelocityChange);
		rigidbody.AddForce((swingClampingForce + distanceClampingForce) / Time.fixedDeltaTime, ForceMode.VelocityChange);

		if (collisionController.colliding)
			rigidbody.velocity = SwordPhysics.StickSwordMovementToCollision(this, rigidbody.velocity);
		
		lastVelocity = rigidbody.velocity;
	}

	public void Rotate(Quaternion rotateTo)
	{
		if (collisionController.colliding)
			return;
		
		Vector3 torque = Quaternions.FromToAngleAxis(rigidbody.rotation, rotateTo);
		
		if (Mathf.Abs(torque.magnitude) > 0.01f)
			rigidbody.AddTorque(torque * (1.0f + angularDamping) * (1.0f + rigidbody.mass * 15f));

		/*
		Vector3 dir = swordController.TipPosition() - swordController.BasePosition();
		Vector3 nextDir = (swordController.TipPosition() + tipForce * Time.fixedDeltaTime) - swordController.BasePosition();
		
		Vector3 axis = Vector3.Cross(dir, nextDir).normalized;
		
		rigidbody.AddTorque(axis * tipForce.magnitude);
		*/
		
		lastAngularVelocity = rigidbody.angularVelocity;
	}
	
	public void Collide(SwordCollision collision, float penetration = 0.0f)
	{
		rigidbody.velocity *= penetration;
		rigidbody.angularVelocity *= penetration;
	}
	
	public void RevertVelocity()
	{
		rigidbody.velocity = lastVelocity;
		rigidbody.angularVelocity = lastAngularVelocity;
	}
	
	public void HandleCollision()
	{
		rigidbody.velocity = SwordPhysics.StickSwordMovementToCollision(this, rigidbody.velocity);
	}
	
	public Vector3 NextPosition()
	{
		return rigidbody.position + rigidbody.velocity * Time.fixedDeltaTime;
	}
		
	public Vector3 NextPosition(Vector3 force)
	{
		return rigidbody.position + (rigidbody.velocity + force) * Time.fixedDeltaTime;
	}
	public Vector3 Position() { return rigidbody.position; }
	public Quaternion Rotation() { return rigidbody.rotation; }
}
