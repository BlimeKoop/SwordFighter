using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vectors : MonoBehaviour
{
	public static Vector3 FlattenVector(Vector3 vector)
	{
		return Vector3.Scale(vector, new Vector3(1f, 0f, 1f));
	}
	
	public static Vector3 FlattenVector(Vector3 vector, Vector3 axis)
	{
		return vector - axis * Vector3.Dot(vector, axis);
	}
}
