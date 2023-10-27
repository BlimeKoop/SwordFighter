using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	private Transform camera;

	private void Start()
	{
		camera = Camera.main.transform;
	}

	void LateUpdate()
    {
		LookAtCamera();
    }
	
	private void LookAtCamera()
	{
		transform.LookAt(camera);
	}
}
