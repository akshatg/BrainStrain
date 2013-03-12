using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class SFXs
{
	public AudioClip BlockHit;
}

public class Musician : MonoBehaviour
{
	public SFXs SFXs;
	public AudioSource Music;
	
	public static SFXs Sounds
	{
		get
		{
			return sounds;
		}
	}
	public static bool MusicOn
	{
		get
		{
			return musicOn;	
		}
		set
		{
			musicOn = value;
			if(musicOn)
			{
				if(!_audio.isPlaying)
				{
					_audio.Play();
				}
			}
			else
			{
				_audio.Pause();	
			}
		}
	}
	public static bool SoundsOn{get; set;}
	public static float MusicVolume
	{
		get
		{
			return musicVolume;	
		}
		set
		{
			musicVolume = value;
			VolumeChanged();
		}
	}
	public static float SoundsVolume
	{
		get
		{
			return soundsVolume;	
		}
		set
		{
			soundsVolume = value;
			VolumeChanged();
		}
	}
	public static float MasterVolume
	{
		get
		{
			return masterVolume;
		}
		set
		{
			masterVolume = value;
			VolumeChanged();
		}
	}
	
	private static SFXs sounds;
	private static bool musicOn;
	private static float musicVolume;
	private static float soundsVolume;
	private static float masterVolume;
	private static AudioSource _audio;
	
	void Start()
	{
		_audio = Music;
		sounds = SFXs;
		//defaults
		MusicOn = true;
		SoundsOn = true;
		MusicVolume = 1f;
		SoundsVolume = 1f;
		MasterVolume = 1f;
	}
	
	public static void PlaySound(AudioClip sound)
	{
		if(SoundsOn)
		{
			_audio.PlayOneShot(sound, SoundsVolume * MasterVolume);
		}
	}
	
	public static void PlaySound(AudioClip sound, Vector3 position)
	{
		if(SoundsOn)
		{
			AudioSource.PlayClipAtPoint(sound, position, SoundsVolume * MasterVolume);
		}
	}
	
	private static void VolumeChanged()
	{
		_audio.volume = MusicVolume * MasterVolume;
	}
}
