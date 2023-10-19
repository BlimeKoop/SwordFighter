using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmIKTargetController : MonoBehaviour
{
	[HideInInspector] public enum State {
		Slowing,
		Slow,
		Quickening,
		Quick
	}
	
	public State state = State.Quick;
	
	private PlayerAnimationController animationController;
	
	private Vector3 slowPosition;
	private Quaternion slowRotation;
	
	private float followFactor = 1.0f;
	private float FollowAmountMin = 0.4f;
	private float QuickenMultiplier = 3.4f;
	private float SlowMultiplier = 3.4f;

	private void Awake()
	{
		followFactor = 1.0f;
	}
	
	public void DoUpdate()
	{
		switch(state)
		{
			case State.Slowing: FinishSlowing(); break;
			case State.Slow: state = State.Quickening; break;
			case State.Quickening: FinishQuickening(); break;
		}
		
		// Debug.Log(followFactor);
		
		transform.position = CalculatePosition();
	}

	private void FinishSlowing()
	{
		followFactor = Mathf.Max(FollowAmountMin, followFactor - Time.deltaTime * SlowMultiplier);
		
		if (followFactor == FollowAmountMin)
			state = State.Slow;
	}

	private void FinishQuickening()
	{
		followFactor = Mathf.Min(followFactor + Time.deltaTime * QuickenMultiplier, 1f);
		
		if (followFactor == 1f)
			state = State.Quick;
	}

	public virtual Vector3 CalculatePosition()
	{
		Vector3 targetPosition = PlayerAnimation.ArmIKTargetPosition(animationController, animationController.playerController);
		
		return Vector3.Lerp(slowPosition, targetPosition, followFactor);
	}
	
	public void Slow()
	{
		state = State.Slowing;
		
		slowPosition = transform.position;
		slowRotation = transform.rotation;
	}

	public void SetAnimationController(PlayerAnimationController animationController) { this.animationController = animationController; }
	public void SetFollowAmount(float followFactor) { this.followFactor = followFactor; }
}
