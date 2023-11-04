using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootColliderSoundEffectManager : SoundEffectManager
{
	private float cooldown = 0.3f;
	private float cooldownTimer;
	
	private void Update()
	{
		cooldownTimer += Time.deltaTime;
	}
	
	private void OnTriggerEnter(Collider col)
	{
		if (col.isTrigger)
			return;
		
		PlayImpact(Random.Range(.015f, .04f));
	}
	
	protected override List<AudioSource> GetImpactSources()
	{
		var sourcesR = new List<AudioSource>();
		
		foreach(var source in impactsSource.GetComponents<AudioSource>())
			sourcesR.Add(source);
			
		return sourcesR;
	}
	
	public override void PlayImpact(float strength01)
	{
		if (cooldownTimer < cooldown)
			return;
		
		strength01 = Mathf.Clamp01(strength01);
		
		var source = impacts[(int) ((impacts.Count - 1) * strength01)];
		
		source.volume = strength01;
		source.Play();
		
		cooldownTimer = 0;
	}
}
