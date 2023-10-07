using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicMeshCutter;

public class PlayerSwordControllerInitialization
{
	public static Rigidbody InitializeRigidbody(SwordPhysicsController physicsController)
	{
		Rigidbody rigidbodyR = (
			physicsController.GetComponent<Rigidbody>() == null ?
			physicsController.gameObject.AddComponent<Rigidbody>() :
			physicsController.GetComponent<Rigidbody>());
		
		rigidbodyR.useGravity = false;
		rigidbodyR.collisionDetectionMode = CollisionDetectionMode.Continuous;
		rigidbodyR.centerOfMass = Vector3.zero;
		rigidbodyR.maxAngularVelocity = 20f;
		rigidbodyR.mass = 0.1f;
		
		rigidbodyR.interpolation = RigidbodyInterpolation.Interpolate;
		
		return rigidbodyR;
	}
	
	public static SpringJoint InitializeSpringJoint(PlayerSwordController playerSwordController)
	{
		SpringJoint springJointR = (
			playerSwordController.GetComponent<SpringJoint>() == null ?
			playerSwordController.gameObject.AddComponent<SpringJoint>() :
			playerSwordController.GetComponent<SpringJoint>());
		
		return springJointR;
	}

	public static SwordCollisionController InitializeCollisionController(PlayerSwordController playerSwordController)
	{
		SwordCollisionController collisionControllerR = playerSwordController.GetComponent<SwordCollisionController>();

		if (collisionControllerR == null)
			collisionControllerR = playerSwordController.gameObject.AddComponent<SwordCollisionController>();
		
		collisionControllerR.SetPlayerController(playerSwordController.playerController);
		
		return collisionControllerR;
	}

	public static SwordPhysicsController InitializePhysicsController(PlayerSwordController playerSwordController)
	{
		SwordPhysicsController physicsControllerR = playerSwordController.GetComponent<SwordPhysicsController>();

		if (physicsControllerR == null)
			physicsControllerR = playerSwordController.gameObject.AddComponent<SwordPhysicsController>();
		
		physicsControllerR.Initialize(playerSwordController);
		return physicsControllerR;
	}

	public static SwordCutterBehaviour InitializeSwordCutterBehaviour(PlayerSwordController playerSwordController)
	{
		SwordCutterBehaviour swordCutterBehaviourR = playerSwordController.GetComponent<SwordCutterBehaviour>();

		if (swordCutterBehaviourR == null)
			swordCutterBehaviourR = playerSwordController.gameObject.AddComponent<SwordCutterBehaviour>();
		
		swordCutterBehaviourR.DefaultMaterial = playerSwordController.cutMaterial;
		swordCutterBehaviourR.Separation = 0.1f;
		
		return swordCutterBehaviourR;
	}
	
	public static void InitializeFractureComponent(Fracture f, PlayerSwordController playerSwordController)
	{
		FractureOptions fo = f.fractureOptions;

		fo.useOrigin = true;
		fo.useAxis = true;
		
		fo.origin = playerSwordController.transform.position;
		fo.axis = Vector3.Cross(
		playerSwordController.physicsController.velocity, playerSwordController.transform.forward).normalized;
		
		fo.asynchronous = true;
	}
}
