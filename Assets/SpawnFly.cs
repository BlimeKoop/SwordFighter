using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class SpawnFly : MonoBehaviour
{
	public Spawner spawner;
	
    public void Spawn()
    {
		if (GameObject.Find("Fly") == null)
		{
			spawner.Spawn(0);
		}
    }
}
