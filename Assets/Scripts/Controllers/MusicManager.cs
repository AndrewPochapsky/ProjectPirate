using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

	public static MusicManager Instance;

	
	public AudioClip MainMenu;
	
	public AudioClip World;

	public AudioClip Battle;

	private AudioSource audioSource;


	private string lastScene = null;


	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		DontDestroyOnLoad(this);

		if(Instance != this && Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
		audioSource = GetComponent<AudioSource>();
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if(lastScene != SceneManager.GetActiveScene().name)
		{
			ChooseClip();
			lastScene = SceneManager.GetActiveScene().name;
		}
	}
	

	private void ChooseClip()
	{
		AudioClip clip = (AudioClip)this.GetType()
            .GetField(SceneManager.GetActiveScene().name)
            .GetValue(this);

        audioSource.clip = clip;
        audioSource.Play();
	}
}
