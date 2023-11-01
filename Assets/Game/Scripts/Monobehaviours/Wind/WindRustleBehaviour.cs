using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RustleBehaviour))]
public class WindRustleBehaviour : WindBehaviour
{
	private RustleBehaviour RustleBehaviour;
	
	private float randomMult;

	public override void Start()
	{
		base.Start();
		
		RustleBehaviour = GetComponent<RustleBehaviour>();
		randomMult = Random.Range(0.2f, 1.5f);
		
		StartCoroutine(GetReady());
	}
	
	private IEnumerator GetReady()
	{
		yield return new WaitUntil(() => ready);
		
		RustleBehaviour.axis = Vectors.SafeCross(Vector3.up, StageManager.Wind, Vector3.forward).normalized;
		RustleBehaviour.intervalDuration = 0.5f * (1.0f - randomMult * 0.3f);
		// StartCoroutine(UpdateWindDirection());
	}
	
	// private IEnumerator UpdateWindDirection();
	
	public override void PerformBehaviour()
	{
		RustleBehaviour.speed = Mathf.Min(StageManager.Wind.magnitude * Time.fixedDeltaTime * 0.06f,  0.1f) * randomMult;
	}
}
