using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class Fly : MonoBehaviour
{
	private RigidbodySynchronizable rb;
	
    // Start is called before the first frame update
    void Start()
    {
		rb = gameObject.GetComponent<RigidbodySynchronizable>();
		
		if (!transform.parent.GetComponent<Alteruna.Avatar>().IsOwner)
		{
			rb.SendData = false;
			return;
		}
		
        StartCoroutine(DoFly());
    }

    private IEnumerator DoFly()
	{
		int timer = 100;
		
		while (timer > 0)
		{
			rb.AddForce(Vector3.up * 0.1f);
			timer--;
			
			yield return null;
		}
		
		timer = 100;
		
		while (timer > 0)
		{
			timer--;
			
			yield return null;
		}
	}
}
