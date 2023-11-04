using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAudioManager : MonoBehaviour
{
	public GameObject impactsSource;
	public GameObject metalImpactsSource;
	public GameObject metalReleasesSource;
	public GameObject slicesSource;
	
	public AudioSource _winSliceSource;
	
	private List<AudioSource> impacts  = new List<AudioSource>();
	private List<AudioSource> metalImpacts  = new List<AudioSource>();
	private List<AudioSource> metalReleases  = new List<AudioSource>();
	private List<AudioSource> slices  = new List<AudioSource>();
	private AudioSource winSliceSource;
	
	private void Awake()
	{
		impacts = GetImpactSources();
		metalImpacts = GetMetalImpactSources();
		metalReleases = GetMetalReleaseSources();
		slices = GetSliceSources();
		
		winSliceSource = _winSliceSource;
	}
	
	private List<AudioSource> GetImpactSources()
	{
		var sourcesR = new List<AudioSource>();
		
		foreach(var source in impactsSource.GetComponents<AudioSource>())
			sourcesR.Add(source);
			
		return sourcesR;
	}

	private List<AudioSource> GetMetalImpactSources()
	{
		var sourcesR = new List<AudioSource>();
		
		foreach(var source in metalImpactsSource.GetComponents<AudioSource>())
			sourcesR.Add(source);
			
		return sourcesR;
	}
	
	private List<AudioSource> GetMetalReleaseSources()
	{
		var sourcesR = new List<AudioSource>();
		
		foreach(var source in metalReleasesSource.GetComponents<AudioSource>())
			sourcesR.Add(source);
			
		return sourcesR;
	}
	
	private List<AudioSource> GetSliceSources()
	{
		var sourcesR = new List<AudioSource>();
		
		foreach(var source in slicesSource.GetComponents<AudioSource>())
			sourcesR.Add(source);
			
		return sourcesR;
	}
	
	public void PlayImpact(float strength01)
	{
		strength01 = Mathf.Clamp01(strength01);
		
		var source = impacts[(int) ((impacts.Count - 1) * strength01)];
		
		source.volume = strength01;
		source.Play();
	}
	
	public void PlayMetalImpact(float strength01)
	{
		strength01 = Mathf.Clamp01(strength01);
		
		var source = metalImpacts[(int) ((metalImpacts.Count - 1) * strength01)];
		
		source.volume = strength01;
		source.Play();
	}
	
	public void PlayMetalRelease(float strength01)
	{
		strength01 = Mathf.Clamp01(strength01);
		
		var source = metalReleases[(int) ((metalReleases.Count - 1) * strength01)];
		
		source.volume = strength01;
		source.Play();
	}
	
	public void PlaySlice(float strength01)
	{
		strength01 = Mathf.Clamp01(strength01);
		
		var source = slices[(int) ((slices.Count - 1) * strength01)];
		
		source.volume = strength01;
		source.Play();
	}

	public void PlayWinSlice(float strength01)
	{
		strength01 = Mathf.Clamp01(strength01);
		
		winSliceSource.volume = strength01;
		winSliceSource.Play();
	}
}
