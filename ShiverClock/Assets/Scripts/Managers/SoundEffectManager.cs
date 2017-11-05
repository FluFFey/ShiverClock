using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager instance;
    private List<AudioSource> _audioSources;
    private float _timeScale = 1.0f;
    private float _previousTimeScale = 1.0f;

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

    public void 
	
	// Update is called once per frame
	void Update () {
		
	}
}
