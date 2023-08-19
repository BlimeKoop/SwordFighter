using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerInitialization : MonoBehaviour
{
	public static PlayerInputController InputController(PlayerController playerController)
	{
		PlayerInputController picReturn = playerController.GetComponent<PlayerInputController>();

		if (picReturn == null)
			picReturn = playerController.gameObject.AddComponent<PlayerInputController>();
		
		return picReturn;
	}
	
	public static PlayerAnimationController AnimationController(PlayerController playerController)
	{
		PlayerAnimationController pacReturn = playerController.GetComponent<PlayerAnimationController>();

		if (pacReturn == null)
			pacReturn = playerController.gameObject.AddComponent<PlayerAnimationController>();
		
		return pacReturn;
	}
	
	public static PlayerCollisionController CollisionController(PlayerController playerController)
	{
		PlayerCollisionController pccReturn = playerController.GetComponent<PlayerCollisionController>();

		if (pccReturn == null)
			pccReturn = playerController.gameObject.AddComponent<PlayerCollisionController>();
		
		return pccReturn;
	}
	
	public static PlayerPhysicsController PhysicsController(PlayerController playerController)
	{
		PlayerPhysicsController ppcReturn = playerController.GetComponent<PlayerPhysicsController>();

		if (ppcReturn == null)
			ppcReturn = playerController.gameObject.AddComponent<PlayerPhysicsController>();
		
		return ppcReturn;
	}
	
	public static PlayerSwordController SwordController(PlayerController playerController)
	{
		Transform sword = playerController.GetSwordModel();
		Transform pivot = new GameObject(sword.gameObject.name + " Pivot").transform;
		
		PlayerSwordController pscReturn = pivot.gameObject.AddComponent<PlayerSwordController>();
		
		return pscReturn;
	}
}
