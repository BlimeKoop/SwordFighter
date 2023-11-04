using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundEffectManager : SoundEffectManager
{
	public GameObject buttonClickSources;
    private void Awake()
	{
		sources[SoundEffects.Type.ButtonClick] = GetSources(buttonClickSources);
	}
	
	protected List<AudioSource> GetSources(GameObject sourcesObj)
	{
		var sourcesR = new List<AudioSource>();
		
		foreach(var source in sourcesObj.GetComponents<AudioSource>())
			sourcesR.Add(source);
			
		return sourcesR;
	}
	
	protected override List<AudioSource> GetImpactSources()
	{
		return null;
	}
	
	public override void PlayImpact(float strength01)
	{
		return;
	}
	
	public void PlayButtonSound()
	{
		sources[SoundEffects.Type.ButtonClick][0].Play();
	}
}
