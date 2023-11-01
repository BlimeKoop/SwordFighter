using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WindBehaviour : MonoBehaviour
{
	protected StageManager StageManager;
	
	protected bool ready;
	
	public virtual void Start()
	{
		StartCoroutine(GetReady());
	}
	
	private IEnumerator GetReady()
	{
		StageManager = FindObjectOfType<StageManager>();
		
		if (StageManager == null)
		{
			yield return null;
			
			StartCoroutine(GetReady());
			yield break;
		}
		
		ready = true;
	}
	
    public virtual void Update()
	{
		if (!ready)
			return;
		
		PerformBehaviour();
	}
	
	public abstract void PerformBehaviour();
}
