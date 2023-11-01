using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustleBehaviour : MonoBehaviour
{
    public float speed { get { return _speed; } set {  _speed = value; currentSpeed = value; } }
	private float _speed;
	
	private float currentSpeed;
	private float restMultiplier;
	
	[HideInInspector]
    public Vector3 axis;
    private Vector3 angularVelocity;
	
	[HideInInspector]
	public Quaternion baseRotation;

    public float intervalDuration = 0.1f;
    private float intervalTimer;
	
	public float restDuration = 1.5f;
	private float restTimer;
	public float restChance = 0.03f;

    private int frameWaitCounter;

    private bool forward = true;
	private bool ready;

    private void Start()
    {
        intervalTimer = intervalDuration / 2;
		baseRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
		if (!ready)
			return;

		float speedNP =  forward ? _speed : -_speed;
		
		currentSpeed = speedNP * restMultiplier;
		
		if (currentSpeed != 0)
			transform.Rotate(axis * currentSpeed, Space.World);
		else
			transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, Time.fixedDeltaTime);
    }
	
	private void Update()
	{
		if (!ready)
		{
			frameWaitCounter++;
			
			ready = frameWaitCounter > 4;
			
			return;
		}
		
		_speed = _speed > 0.01f ? _speed * (1f - Time.deltaTime * 6f) : 0f;
		
		if (restTimer > 0)
		{
			restMultiplier = Mathf.Max(0.2f, restMultiplier - Time.deltaTime * 0.5f);
		}
		else
		{
			restMultiplier = Mathf.Min(restMultiplier + Time.deltaTime * 6f, 1.0f);
		}
		
        restTimer -= Time.deltaTime;
		
		if (restTimer < 0)
		{
			if (Random.Range(0f, 1f) < restChance)
			{
				restTimer = restDuration;
			}
		}
		
        intervalTimer += Time.deltaTime;

        if (intervalTimer > intervalDuration)
        {
            forward = !forward;

            intervalTimer = 0;
        }
	}
}
