using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
	public Vector3 axis = new Vector3(0f, 1f, 0f);
	
	public float speed = 7f;
	
    void Update()
	{
		transform.Rotate(axis * speed * Time.deltaTime);
	}
}
