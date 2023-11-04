using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootColliderSound : MonoBehaviour
{
	public SoundEffectManager footSoundEffectManager;
	
	private void OnTriggerEnter(Collider col)
	{
		if (col.isTrigger)
			return;
		
		footSoundEffectManager.PlayImpact(.3f);
	}
}
