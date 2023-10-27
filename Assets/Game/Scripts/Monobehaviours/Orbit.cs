using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
	public Transform target;
	public Transform pivot;

	public float distance = 15f;
	public float height = 5f;

    void LateUpdate()
    {
		if (pivot == null)
		{
			Debug.Log("Pivot is null");
			return;
		}
		
        transform.position = pivot.position - pivot.forward * distance + Vector3.up * height;
    }
}
