using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivotController : MonoBehaviour
{
	private Transform followTarget;
	
	[Range(0,1)] public float followSpeed = 0.4f;
	[Range(0,1)] public float directionChangeSpeed = 0.5f;
	
	public float DirectionChangeDelay = 0f;
	
	public float DirectionCooldownMin = 0.1f;
	public float DirectionCooldownMax = 0.1f;
	
	private float autoRotateEaseDuration = 0.2f;
	private float rotateEaseDuration = 0.0f;

	private float directionChangeDuration;

	private float directionChangeCooldownTimer;
	private float directionChangeTimer;
	
	private float directionChangeDegrees;
	private float autoRotateDegrees;
	private float rotateDegrees;
	
	private bool zeroAutoRotateDegrees;
	private bool zeroRotateDegrees;
	
	private float autoRotateTimer;
	private float rotateTimer;
	
	private Vector3 lastDirection;
	
	[HideInInspector]
	public Vector3 targetDirection;
	
	private bool initialized;
	private bool changingDirection, autoRotate, rotate;
	
	public bool enabled;
	
	private void OnEnable()
	{
		enabled = true;
	}
	
	private void OnDisable()
	{
		enabled = false;
	}
	
    public void Initialize(Transform _followTarget)
    {
        followTarget = _followTarget;
		
		transform.position = followTarget.position;
		transform.forward = followTarget.forward;
		
		targetDirection = transform.forward;
		
		initialized = true;
    }

    public void DoLateUpdate()
    {
		if (!enabled || !initialized)
			return;
		
        transform.position = Vector3.Lerp(
		transform.position, followTarget.position, followSpeed * 0.1f * Time.deltaTime * 80f);

		HandleDirectionChange();
		HandleAutoRotation();
		HandleRotation();

		directionChangeCooldownTimer += Time.deltaTime;
		directionChangeTimer += Time.deltaTime;
		autoRotateTimer += Time.deltaTime;
		rotateTimer += Time.deltaTime;
		
		zeroAutoRotateDegrees = true;
		zeroRotateDegrees = true;
	}
	
	private void HandleDirectionChange()
	{
		if (directionChangeDegrees != 0)
		{
			if (directionChangeTimer < DirectionChangeDelay)
				return;
			
			float changeCompletionFactor = Mathf.Min((
				directionChangeTimer - DirectionChangeDelay) / directionChangeDuration,
				1.0f);
			float t = 1.0f - Mathf.Pow(1.0f - changeCompletionFactor, 2);
			// float smoothingFactor = (1.0f - (Mathf.Abs(changeCompletionFactor - 0.3f) / 0.7f)) * 2;
			
			transform.forward = Vector3.RotateTowards(
				lastDirection,
				targetDirection,
				directionChangeDegrees * t * Mathf.Deg2Rad,
				1.0f);
			
			if (t == 1f)
			{
				transform.forward = targetDirection;
				
				directionChangeDegrees = 0;
				directionChangeCooldownTimer = 0;
			}
		}
		else
		{
			changingDirection = false;
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
			
			float multiplier = Mathf.Clamp(autoRotateTimer / Mathf.Max(0.01f, autoRotateEaseDuration), 0.2f, 1.0f);
			transform.Rotate(Vector3.up, autoRotateDegrees * multiplier * Time.deltaTime);
			
			if (zeroAutoRotateDegrees)
				autoRotateDegrees = ZeroRotationDegrees(autoRotateDegrees);
		}
		else
			autoRotate = false;
	}
	
	private void HandleRotation()
	{
		if (rotateDegrees != 0)
		{
			if (!rotate)
			{
				rotateTimer = 0;
				rotate = true;
			}
			
			float multiplier = Mathf.Clamp(rotateTimer / Mathf.Max(0.01f, rotateEaseDuration), 0.2f, 1.0f);
			transform.Rotate(Vector3.up, rotateDegrees * multiplier * Time.deltaTime);
			
			if (zeroRotateDegrees)
				rotateDegrees = ZeroRotationDegrees(rotateDegrees);
		}
		else
			rotate = false;
	}
	
	private float ZeroRotationDegrees(float deg, float deltaTimeMult = 16.0f)
	{
		float change = (0 - deg) * Time.deltaTime * deltaTimeMult;
		
		// if (Mathf.Abs(change) < 0.04f)
			// change = 0.04f * Math.FloatN1P1(change);
		
		if (deg + change > 0 != deg > 0)
			return 0f;
		
		return deg + change;
			
	}
	
	public void ChangeDirection(Vector2 input)
	{
		if (input.magnitude == 0)
			return;

		Vector3 _lastDirection = transform.forward;
		Vector3 _targetDirection = (transform.right * input.x + transform.forward * input.y).normalized;
		
		float _degrees = Vector3.Angle(_lastDirection, _targetDirection);
		
		/*
		if (directionChangeCooldownTimer < (
				_degrees < 160f ?
				DirectionCooldownMin :
				Mathf.Lerp(DirectionCooldownMin, DirectionCooldownMax, _degrees / 180f)))
			return; */
		
		lastDirection = _lastDirection;
		targetDirection = _targetDirection;
		directionChangeDegrees = _degrees;
		
		directionChangeDuration = Mathf.Lerp(
			0.5f * (1f - directionChangeSpeed),
			1f - directionChangeSpeed,
			directionChangeDegrees / 180f);

		if (changingDirection)
			directionChangeTimer = DirectionChangeDelay;
		else
			directionChangeTimer = 0;
		
		changingDirection = true;
	}
	
	public void SetAutoRotateDegrees(float degrees)
	{
		if (changingDirection)
			return;
		
		if (Mathf.Abs(degrees) < 0.01f)
			return;
		
		zeroAutoRotateDegrees = false;
		
		if (autoRotate && degrees > 0f != autoRotateDegrees > 0f)
		{
			autoRotateDegrees = 0;
			autoRotate = false;
			
			return;
		}
		
		if (Mathf.Abs(degrees) > Mathf.Abs(autoRotateDegrees))
			autoRotateDegrees = degrees;
	}
	
	public void Rotate(float degrees)
	{
		if (changingDirection)
			return;
		
		if (Mathf.Abs(degrees) < 0.01f)
			return;
		
		zeroRotateDegrees = false;
		
		if (rotate && degrees > 0f != rotateDegrees > 0f)
		{
			rotateDegrees = 0;
			rotate = false;
			
			return;
		}
		
		if (Mathf.Abs(degrees) > 300f)
			degrees = 700f * Math.FloatN1P1(degrees);
		
		if (Mathf.Abs(degrees) > Mathf.Abs(rotateDegrees))
			rotateDegrees = degrees;
	}
}
