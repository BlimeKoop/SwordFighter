using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivotController : MonoBehaviour
{
	private Transform followTarget;
	
	[Range(0,1)] public float followSpeed = 0.4f;
	public float rotateSpeed = 0.5f;
	
	public float DirectionChangeDelay = 0.05f;
	public float AutoRotateDelay = 0.2f;
	public float AutoRotateEaseDuration = 0.2f;
	
	public float DirectionCooldownMin = 0.1f;
	public float DirectionCooldownMax = 0.1f;
	
	private float directionChangeDuration;

	private float directionChangeCooldownTimer;
	private float directionChangeTimer;
	
	private float rotateDegrees;
	private float autoRotateDegrees;
	
	private float autoRotateTimer;
	
	private Vector3 lastDirection;
	
	[HideInInspector]
	public Vector3 targetDirection;
	
	private bool initialized;
	private bool rotate, autoRotate;
	[HideInInspector] public bool rotating;
	
    public void Initialize(GameObject _followTarget)
    {
        followTarget = _followTarget.transform;
		
		transform.position = followTarget.position;
		transform.forward = followTarget.forward;
		
		targetDirection = transform.forward;
		
		initialized = true;
    }

    public void DoLateUpdate()
    {
		if (!initialized)
			return;
		
        transform.position = Vector3.Lerp(
		transform.position, followTarget.position, followSpeed * 0.1f * Time.deltaTime * 80f);

		HandleRotation();		
		HandleAutoRotation();

		directionChangeCooldownTimer += Time.deltaTime;
		directionChangeTimer += Time.deltaTime;
		autoRotateTimer += Time.deltaTime;
	}
	
	private void HandleRotation()
	{
		if (rotate && directionChangeTimer > DirectionChangeDelay)
		{
			rotate = false;
			rotating = true;
			
			directionChangeTimer = 0;
		}
		
		if (rotating)
			Rotate();
	}
	
	private void Rotate()
	{		
		float changeCompletionFactor = directionChangeTimer / directionChangeDuration;
		float t = Mathf.Min(1.0f - Mathf.Pow(1.0f - changeCompletionFactor, 2), 1.0f);
		// float smoothingFactor = (1.0f - (Mathf.Abs(changeCompletionFactor - 0.3f) / 0.7f)) * 2;
		
		transform.forward = Vector3.RotateTowards(
			lastDirection,
			targetDirection,
			rotateDegrees * t * Mathf.Deg2Rad,
			1.0f);
		
		if (directionChangeTimer > directionChangeDuration)
		{
			rotating = false;
			transform.forward = targetDirection;
			
			directionChangeCooldownTimer = 0;
			
			return;
		}
	}
	
	private void HandleAutoRotation()
	{
		if (autoRotateDegrees != 0)
		{
			if (!autoRotate)
			{
				autoRotateTimer = 0;
				autoRotate = true;
			}
			
			if (autoRotateTimer > AutoRotateDelay)
			{
				float multiplier = Mathf.Min((autoRotateTimer - AutoRotateDelay) / AutoRotateEaseDuration, 1.0f);
				transform.Rotate(Vector3.up, autoRotateDegrees * multiplier * Time.deltaTime);
			}
			
			autoRotateDegrees = 0;
		}
		else
			autoRotate = false;
	}
	
	public void ChangeDirection(Vector2 input)
	{
		if (input.magnitude == 0)
			return;
		
		if (rotating)
			return;
		
		Vector3 _lastDirection = transform.forward;
		Vector3 _targetDirection = (transform.right * input.x + transform.forward * input.y).normalized;
		float _rotateDegrees = Vector3.Angle(_lastDirection, _targetDirection);
		
		float cooldown = (
			_rotateDegrees < 160f ?
			DirectionCooldownMin :
			Mathf.Lerp(DirectionCooldownMin, DirectionCooldownMax, _rotateDegrees / 180f));
		
		if (directionChangeCooldownTimer < cooldown)
			return;
		
		lastDirection = _lastDirection;
		targetDirection = _targetDirection;
		rotateDegrees = _rotateDegrees;
		
		directionChangeDuration = Mathf.Lerp(0.5f * (1f - rotateSpeed), 1f - rotateSpeed, rotateDegrees / 180f);
		directionChangeTimer = 0;
		
		rotate = true;
	}
	
	public void Rotate(float degrees)
	{
		if (rotating)
			return;
		
		if (Mathf.Abs(degrees) < 0.01f)
			degrees = 0f;
		
		autoRotateDegrees = degrees;
	}
}
