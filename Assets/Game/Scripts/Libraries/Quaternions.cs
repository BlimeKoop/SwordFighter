using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quaternions
{
	public static Vector3 FromToAngleAxis(Quaternion quatFrom, Quaternion quatTo)
	{
		(quatTo * Quaternion.Inverse(quatFrom)).ToAngleAxis(out float angle, out Vector3 axis);
		
		if (angle > 180f)
			angle = -(360f - angle);
		
		return axis.normalized * angle;
	}
}
