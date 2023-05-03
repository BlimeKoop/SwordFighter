using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbodies
{
	public static Transform ApproximateLastTransform(Rigidbody rigidbody)
	{
		Transform lastTransform = new GameObject("TempTransform").transform;
		lastTransform.gameObject.AddComponent<DestroyAtEndOfFrame>();
		
		lastTransform.position = rigidbody.position - rigidbody.velocity * Time.fixedDeltaTime;
		lastTransform.rotation = rigidbody.rotation * Quaternion.Inverse(
		Quaternion.AngleAxis(rigidbody.angularVelocity.magnitude * Time.fixedDeltaTime, rigidbody.angularVelocity.normalized));
		
		return lastTransform;
	}

	public static Transform ApproximateNextTransform(Rigidbody rigidbody)
	{
		Transform nextTransform = new GameObject("TempTransform").transform;
		nextTransform.gameObject.AddComponent<DestroyAtEndOfFrame>();
		
		nextTransform.position = rigidbody.position + rigidbody.velocity * Time.fixedDeltaTime;
		nextTransform.rotation = rigidbody.rotation * Quaternion.AngleAxis(
		rigidbody.angularVelocity.magnitude * Time.fixedDeltaTime, rigidbody.angularVelocity.normalized);
		
		return nextTransform;
	}
}
