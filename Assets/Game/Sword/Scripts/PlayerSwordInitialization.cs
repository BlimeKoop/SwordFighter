using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicMeshCutter;
using Photon.Pun;

public class PlayerSwordInitialization
{
	public static PhotonRigidbodyView RigidbodyView(PlayerController playerController)
	{
		Transform sword = playerController.sword;
		sword.TryGetComponent<PhotonRigidbodyView>(out PhotonRigidbodyView rigidbodyViewR);
		
		if (rigidbodyViewR == null)
			rigidbodyViewR = sword.gameObject.AddComponent<PhotonRigidbodyView>();
		
		return rigidbodyViewR;
	}
	
	public static Rigidbody Rigidbody(PlayerController playerController)
	{
		Transform sword = playerController.sword;
		
		sword.TryGetComponent<Rigidbody>(out Rigidbody rigidbodyR);
		
		if (rigidbodyR == null)
			rigidbodyR = sword.gameObject.AddComponent<Rigidbody>();
		
		rigidbodyR.useGravity = false;
		rigidbodyR.collisionDetectionMode = CollisionDetectionMode.Continuous;
		rigidbodyR.centerOfMass = Vector3.zero;
		rigidbodyR.maxAngularVelocity = 20f;
		rigidbodyR.mass = 0.05f;
		rigidbodyR.drag = 15.0f;
		rigidbodyR.angularDrag = 35.0f;
		
		rigidbodyR.interpolation = RigidbodyInterpolation.Interpolate;
		
		return rigidbodyR;
	}
	
	public static SwordCollisionController CollisionController(PlayerSwordController swordController)
	{
		Transform sword = swordController.playerController.sword;
		
		SwordCollisionController collisionControllerR = sword.GetComponent<SwordCollisionController>();

		if (collisionControllerR == null)
			collisionControllerR = sword.gameObject.AddComponent<SwordCollisionController>();

		collisionControllerR.SetPlayerController(swordController.playerController);
		collisionControllerR.Initialize(swordController);
		
		return collisionControllerR;
	}

	public static SwordPhysicsController PhysicsController(PlayerSwordController swordController)
	{
		SwordPhysicsController physicsControllerR = new SwordPhysicsController();
		
		physicsControllerR.Initialize(swordController);
		
		return physicsControllerR;
	}

	public static SwordCutterBehaviour SwordCutterBehaviour(PlayerSwordController swordController)
	{
		Transform sword = swordController.playerController.sword;
		
		SwordCutterBehaviour swordCutterBehaviourR = sword.GetComponent<SwordCutterBehaviour>();

		if (swordCutterBehaviourR == null)
			swordCutterBehaviourR = sword.gameObject.AddComponent<SwordCutterBehaviour>();
		
		swordCutterBehaviourR.Separation = 0.1f;
		
		return swordCutterBehaviourR;
	}
	
	public static void FractureComponent(Fracture f, PlayerSwordController swordController)
	{
		FractureOptions fo = f.fractureOptions;

		fo.useOrigin = true;
		fo.useAxis = true;
		
		fo.origin = swordController.playerController.sword.position;
		fo.axis = swordController.playerController.sword.right;
		
		fo.asynchronous = true;
	}
}
