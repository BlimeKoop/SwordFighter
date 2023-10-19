using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects
{
	Transform proxy = new GameObject("Objects Proxy").transform;
	
	/// <summary>
	/// Returns the box collider's extents along object's axises, in world scale.
	/// </summary>

    public static Vector3 BoxColliderExtents(BoxCollider col, Transform axisReference)
	{
		if (col == null)
			return Vector3.zero;
		
		Vector3 center = col.transform.TransformPoint(col.center);
		Vector3 toMax = col.transform.TransformPoint(col.center + col.size / 2) - center;

		return new Vector3(
			Mathf.Abs(Vector3.Dot(axisReference.right, toMax)),
			Mathf.Abs(Vector3.Dot(axisReference.up, toMax)),
			Mathf.Abs(Vector3.Dot(axisReference.forward, toMax)));
	}
	
	/// <summary>
	/// Returns the renderer's extents along object's axises, in world scale.
	/// </summary>

    public static Vector3 RendererExtents(Renderer rend, Transform axisReference)
	{
		if (rend == null)
			return Vector3.zero;
		
		Vector3 toMax = rend.transform.TransformPoint(rend.localBounds.extents) - rend.bounds.center;

		return new Vector3(
			Mathf.Abs(Vector3.Dot(axisReference.right, toMax)),
			Mathf.Abs(Vector3.Dot(axisReference.up, toMax)),
			Mathf.Abs(Vector3.Dot(axisReference.forward, toMax)));
	}
	
	public static T GetComponentInHeirarchy<T>(GameObject obj)
	{
		var found = obj.GetComponentInChildren<T>();
		
		if (obj != null)
			return found;
		
		return obj.GetComponentInParent<T>();
	}
	
	public static T GetComponentInHeirarchy<T>(Component component)
	{
		var found = component.GetComponentInChildren<T>();
		
		if (found != null)
			return found;
		
		return component.GetComponentInParent<T>();
	}
}
