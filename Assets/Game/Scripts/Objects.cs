using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects
{
	/// <summary>
	/// Returns the box collider's extents along object's axises, in world scale.
	/// </summary>

    public static Vector3 BoxColliderExtents(GameObject obj)
	{
		BoxCollider col = obj.GetComponentInChildren<BoxCollider>();
		
		if (col == null)
			return Vector3.zero;
		
		Transform objT = obj.transform;
		Transform colT = col.transform;
		
		Vector3 boxSize = col.size;
		Vector3 max = colT.TransformPoint(boxSize / 2);
		Vector3 toMax = max - colT.position;

		Vector3 extentsR = new Vector3();
		extentsR.x = Mathf.Abs(Vector3.Dot(objT.right, toMax));
		extentsR.y = Mathf.Abs(Vector3.Dot(objT.up, toMax));
		extentsR.z = Mathf.Abs(Vector3.Dot(objT.forward, toMax));

		return extentsR;
	}
}
