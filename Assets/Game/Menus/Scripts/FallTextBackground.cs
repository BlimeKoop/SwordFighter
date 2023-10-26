using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTextBackground : MonoBehaviour
{
	public float speed = 0.3f;
	
	private float resetDelay = 20f;
	private float resetTimer;
	
	private Transform fallText;

    private void Start()
    {
        fallText = transform.Find("Mask").Find("Fall Text");
    }

    private void Update()
    {
		if (resetTimer > resetDelay)
			fallText.position += Vector3.up * 100f;
			
        fallText.position -= Vector3.up * speed * Time.deltaTime * 0.5f;
		
		resetTimer += Time.deltaTime;
    }
}
