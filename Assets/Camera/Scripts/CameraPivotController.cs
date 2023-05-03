using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivotController : MonoBehaviour
{
	public GameObject _followTarget;
	private Transform followTarget;
	
	private Vector3 targetDirection;
	
	[Range(0,1)]
	private float followSpeed = 0.4f;
	
	[Range(0,1)]
	private float _rotateSpeed = 0.4f;
	private float rotateSpeedMax;
	private float rotateSpeed;
	
	private bool initialized;
	private int rotateLRN1P1 = 1;
	
    public void Initialize()
    {
        followTarget = _followTarget.transform;
		targetDirection = followTarget.forward;
		
		transform.position = followTarget.position;
		transform.forward = targetDirection;
		
		initialized = true;
    }

    private void LateUpdate()
    {
		if (!initialized)
			return;
		
		float forwardDifference = Vector3.Angle(transform.forward, targetDirection) / 180f;
		float inverseExponent = 1.0f - Mathf.Pow(1.0f - forwardDifference, 2);
		
		rotateSpeed = rotateSpeedMax * inverseExponent;
		
        transform.position = Vector3.Lerp(transform.position, followTarget.position, followSpeed * 0.1f * Time.deltaTime * 80f);

		transform.forward = Vector3.Lerp(
			transform.forward, transform.right * rotateLRN1P1, rotateSpeed * 0.5f * Time.deltaTime * 80f);
    }

	public bool ForwardAligned()
	{
		return Vector3.Dot(transform.forward, targetDirection) > 0.97f;
	}

	public void SetTargetDirection(Vector3 targetDirection)
	{
		float rDot = Vector3.Dot(transform.right, targetDirection);
		float fDot = Vector3.Dot(transform.forward, targetDirection);
		
		if (fDot > -0.99f)
			rotateLRN1P1 = rDot > 0 ? 1 : -1;
		
		rotateSpeedMax = _rotateSpeed * (0.8f + 0.2f * (1.0f - fDot));
		
		this.targetDirection = targetDirection;
	}
}
