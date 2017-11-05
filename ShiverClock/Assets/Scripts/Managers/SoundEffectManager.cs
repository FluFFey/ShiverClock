using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager instance;
    private List<AudioSource> _audioSources;
    private float _timeScale = 1.0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            DestroyObject(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    public AudioSource findFreeAudioSource()
    {
        foreach (var fxSource in _audioSources)
        {
            if (!fxSource.isPlaying)
            {
                return fxSource;
            }
        }
        //no free audiosource
        return default(AudioSource);
    }

    public bool playSoundEffect(AudioClip clip, float pitchRange = 0.0f)
    {
        AudioSource source = findFreeAudioSource();
        if (source != default(AudioSource)) //found available source
        {
            source.pitch = Random.Range(1.0f - pitchRange * 0.01f, 1.0f + pitchRange * 0.01f);
            source.clip = clip;
            source.Play();

            if (!_audioSources.Contains(source))
            {
                _audioSources.Add(source);
            }
            return true;
        }
        return false;
    }

    public void updatePitchBasedOnGameTimescale(float newTimeScale)
    {
        if (newTimeScale != _timeScale)
        {
            // Apply effect
            foreach (var fxSource in _audioSources)
            {
                if (fxSource.isPlaying)
                {
                    float pitchToUse = fxSource.pitch;
                    pitchToUse = pitchToUse / _timeScale * newTimeScale; // unde previous change, then apply the new one

                    fxSource.pitch = pitchToUse;
                }
            }

            // Store for later
            _timeScale = newTimeScale;
        }
    }
	
	// Update is called once per frame
	void Update () {
        updatePitchBasedOnGameTimescale(
            MyGameManager.instance.timeScale
            );
	}
}
