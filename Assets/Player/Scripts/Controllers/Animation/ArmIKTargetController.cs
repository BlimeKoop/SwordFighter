using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmIKTargetController : MonoBehaviour
{
	public Transform sword;

	private PlayerAnimationController animationController;
	
	private bool locking;
	private bool unlocking;
	
	private Vector3 lockPosition;
	private Quaternion lockRotation;
	
	private float followAmount = 1.0f;
	private float FollowAmountMin = 0.55f;

	private void Awake()
	{
		followAmount = 1.0f;
	}
	
	private void Update()
	{
		Vector3 armBonePosition = animationController.GetArmBone(true).position;
		Vector3 targetPosition = targetPosition = animationController.ArmIKTargetPosition();
		
		if (locking)
			StartUnlock();
		
		if (unlocking)
			Unlock();
		
		if (!locking && !unlocking)
		{
			lockPosition = transform.position;
			lockRotation = transform.rotation;
		}

		transform.position = Vector3.Lerp(lockPosition, targetPosition, followAmount);
		
		Quaternion targetRotation = Quaternion.LookRotation(transform.position - armBonePosition);
		
		transform.rotation = Quaternion.Lerp(lockRotation, targetRotation, followAmount);
	}
	
	private void StartUnlock()
	{
		followAmount = Mathf.Max(FollowAmountMin, followAmount - Time.deltaTime * 3.4f);
		
		if (followAmount == FollowAmountMin)
		{
			locking = false;
			unlocking = true;
		}
	}
	
	private void Unlock()
	{
		followAmount = Mathf.Min(followAmount + Time.deltaTime * 2f, 1f);
		
		if (followAmount == 1f)
			unlocking = false;
	}
	
	public bool GetLocked() { return locking || unlocking; }
	
	public void Lock()
	{
		if (unlocking)
			return;
		
		locking = true;
	}
	
	// public void Unlock() { locked = false; }
	
	public void SetSword(Transform sword) { this.sword = sword; }
	public void SetAnimationController(PlayerAnimationController animationController) { this.animationController = animationController; }
	public void SetFollowAmount(float followAmount) { this.followAmount = followAmount; }
}
