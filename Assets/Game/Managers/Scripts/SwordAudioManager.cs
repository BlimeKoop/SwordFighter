using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAudioManager : MonoBehaviour
{
    public List<AudioSource> _impacts;			private static List<AudioSource> impacts;
    public List<AudioSource> _releases;		private static List<AudioSource> releases;
	
	public AudioSource _fleshSliceSource;		private static AudioSource fleshSliceSource;
	public AudioSource _niceSliceSource;		private static AudioSource niceSliceSource;
	
	private void Awake()
	{
		impacts = _impacts;
		releases = _releases;
		
		fleshSliceSource = _fleshSliceSource;
		niceSliceSource = _niceSliceSource;
	}
	
	public static void PlayImpact(float strength01)
	{
		strength01 = Mathf.Clamp01(strength01);
		
		var source = impacts[(int) ((impacts.Count - 1) * strength01)];
		
		source.volume = strength01;
		source.Play();
	}
	
	public static void PlayRelease(float strength01)
	{
		strength01 = Mathf.Clamp01(strength01);
		
		var source = releases[(int) ((releases.Count - 1) * strength01)];
		
		source.volume = strength01;
		source.Play();
	}
	
	public static void PlayFleshSlice(float strength01)
	{
		strength01 = Mathf.Clamp01(strength01);
		
		fleshSliceSource.volume = strength01;
		fleshSliceSource.Play();
	}

	public static void PlayNiceSlice(float strength01)
	{
		strength01 = Mathf.Clamp01(strength01);
		
		niceSliceSource.volume = strength01;
		niceSliceSource.Play();
	}
}
