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
	
	public static Vector3 SafeCross(Vector3 a, Vector3 b, Vector3 fallBack)
	{
		if (Vector3.Dot(a.normalized, b.normalized) < 1.0f)
			return Vector3.Cross(a, b);
		
		return fallBack;
	}
}
