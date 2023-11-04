using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoundEffectManager : MonoBehaviour
{
	public GameObject impactsSource;
	protected List<AudioSource> impacts  = new List<AudioSource>();
	protected Dictionary<SoundEffects.Type, List<AudioSource>> sources = new Dictionary<SoundEffects.Type, List<AudioSource>>();

	private void Awake()
	{
		impacts = GetImpactSources();
	}
	
	protected abstract List<AudioSource> GetImpactSources();
	
	public abstract void PlayImpact(float strength01);
}