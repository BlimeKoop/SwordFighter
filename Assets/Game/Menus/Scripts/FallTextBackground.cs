using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTextBackground : MonoBehaviour
{
	public float speed = 0.3f;
	
	private float resetDelay = 30f;
	private float resetTimer;
	
	private float startingY;
	
	private Transform fallText;

    private void Start()
    {
        fallText = transform.Find("Mask").Find("Fall Text");
    }

    private void Update()
    {
		if (resetTimer > resetDelay)
		{
			Vector3 position = fallText.position;
			position.y = startingY;
			
			fallText.position = position;
		
			resetTimer = 0;
		}
		
		fallText.Translate(Vector3.up * -speed * 0.5f * Time.deltaTime);

		resetTimer += Time.deltaTime;
    }
}
