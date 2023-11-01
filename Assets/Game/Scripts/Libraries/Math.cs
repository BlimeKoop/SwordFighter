using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math
{
	public static int FloatN1P1(float f)
	{
		if (f > 0.0f)
			return 1;
		
		return -1;
	}
	
	public static float SnappedLerp(float a, float b, float t)
	{
		return
		(
			Mathf.Abs(b - a) > 0.01f  ?
			Mathf.Lerp(a, b, t) :
			b
		);
	}
}
