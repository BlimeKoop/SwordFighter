using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchAudioManager : MonoBehaviour
{
	public AudioSource _themeSource; private static AudioSource themeSource;
	public AudioSource _loopSource; private static AudioSource loopSource;
	
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		
		if (GetComponent<AudioListener>() != null)
			Destroy(GetComponent<AudioListener>());

		themeSource = _themeSource;
		loopSource = _loopSource;
	}

	public static void PlayTheme()
	{
		themeSource.Play();
	}
	
	public static void PlayMusic()
	{
		loopSource.Play();
	}

	public static void StopMusic()
	{
		loopSource.Stop();
	}
}
