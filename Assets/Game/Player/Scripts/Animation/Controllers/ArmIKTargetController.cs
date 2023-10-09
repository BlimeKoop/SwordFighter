using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmIKTargetController : MonoBehaviour
{
	private PlayerAnimationController animationController;
	
	private bool locking;
	private bool unlocking;
	
	private Vector3 lockPosition;
	private Quaternion lockRotation;
	
	private float followFactor = 1.0f;
	private float FollowAmountMin = 0.55f;

	private void Awake()
	{
		followFactor = 1.0f;
	}
	
	public void DoUpdate()
	{
		if (locking)
			FinishLocking();
		
		if (unlocking)
			FinishUnlocking();
		
		transform.position = CalculatePosition();
	}

	public virtual Vector3 CalculatePosition()
	{
		Vector3 targetPosition = PlayerAnimation.ArmIKTargetPosition(animationController, animationController.playerController);
		
		return Vector3.Lerp(lockPosition, targetPosition, followFactor);
	}

	public void Lock()
	{
		locking = true;
		
		SetLockData();
	}
	
	private bool FinishLocking()
	{
		followFactor = Mathf.Max(FollowAmountMin, followFactor - Time.deltaTime * 3.2f);
		
		if (followFactor == FollowAmountMin)
			StartUnlocking();
		
		return !locking;
	}
	
	private void StartUnlocking()
	{
		locking = false;
		unlocking = true;
	}

	private void FinishUnlocking()
	{
		followFactor = Mathf.Min(followFactor + Time.deltaTime * 2.1f, 1f);
		
		if (followFactor == 1f)
			unlocking = false;
	}

	private void SetLockData()
	{
		lockPosition = transform.position;
		lockRotation = transform.rotation;
	}

	public void SetAnimationController(PlayerAnimationController animationController) { this.animationController = animationController; }
	public void SetFollowAmount(float followFactor) { this.followFactor = followFactor; }

	public bool GetLocked() { return locking || unlocking; }
}
