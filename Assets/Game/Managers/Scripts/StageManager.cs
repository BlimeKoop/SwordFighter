using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Vector3 Wind = new Vector3(100f, 50f, 0);
	
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
