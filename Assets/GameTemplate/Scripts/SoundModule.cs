using System.Collections.Generic;
using UnityEngine;

public class SoundModule : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public Sound[] sounds;

    private Dictionary<string, AudioClip> soundDictionary;
    private AudioSource audioSource;
    private AudioSource audioSourceForMusic;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSourceForMusic = gameObject.AddComponent<AudioSource>();
        soundDictionary = new Dictionary<string, AudioClip>();

        // mute through code
        audioSource.volume = .5f;
        audioSourceForMusic.volume = .35f;

        foreach (Sound sound in sounds)
        {
            soundDictionary[sound.name] = sound.clip;
        }

        print("SoundManager is On!");
    }

    public void PlaySound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            audioSource.PlayOneShot(soundDictionary[soundName]);
        }
        else
        {
            Debug.LogWarning("SoundManager: Sound did not found - " + soundName);
        }
    }

    public void PlayMusic(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            if (audioSourceForMusic.clip != null)
            {
                if (soundName == audioSourceForMusic.clip.name)
                {
                    Debug.Log("SoundManager: Same music clip is already playing: " + soundName);
                    return;
                }
                else
                {
                    Debug.Log("SoundManager: New music clip is started: " + soundName);
                    audioSourceForMusic.Stop();
                    audioSourceForMusic.clip = soundDictionary[soundName];
                    audioSourceForMusic.Play();
                }
            }
            else
            {
                //audioSourceForMusic.Stop();
                Debug.Log("SoundManager: Started music clip: " + soundName);
                audioSourceForMusic.clip = soundDictionary[soundName];
                audioSourceForMusic.Play();
            }
        }
        else
        {
            Debug.LogWarning("SoundManager: Sound did not found - " + soundName);
        }
    }
}