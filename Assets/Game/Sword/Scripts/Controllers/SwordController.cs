using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
	public Vector3 velocity;
    
	public void Move(float multiplier = 1.0f)
	{
		transform.position += velocity * multiplier;
	}
}
