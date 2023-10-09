using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAtEndOfFrame : MonoBehaviour
{
    void Update()
    {
        StartCoroutine(Destroy());
    }
	
	private IEnumerator Destroy()
	{
		yield return new WaitForEndOfFrame();
		
		Destroy(gameObject);
	}
}
