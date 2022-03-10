using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private Sound currSound;

    private Sound currS;

    public Sound CurrSound { get { return currSound; } }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        FindSound(name);

        if (currS.loop)
        {
            currSound = currS;
            Debug.Log("Playing music " + name);
        }

        currS.source.Play();
    }

    public void StopPlaying(string sound)
    {
        FindSound(sound);

        currS.source.Stop();
    }

    public void Pause(string sound)
    {
        FindSound(sound);

        currS.source.Pause();
    }

    public void UnPause(string sound)
    {
        FindSound(sound);

        currS.source.UnPause();
    }

    private void FindSound(string sound)
    {
        currS = Array.Find(sounds, item => item.name == sound);
        if (currS == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }
    }
}
