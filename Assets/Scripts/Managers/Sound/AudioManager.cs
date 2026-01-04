using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {   
        // Check if there is already an instance of the AudioManager
        if (instance == null)
        {
            instance = this;
            // Don't destroy the AudioManager when a new scene is loaded
            DontDestroyOnLoad(gameObject);

            // Add AudioSources to the sounds/Elements
            foreach (Sound s in sounds)
            {
                if (s != null && s.clip != null)
                {
                    s.source = gameObject.AddComponent<AudioSource>();
                    s.source.clip = s.clip;
                    s.source.volume = s.volume;
                    s.source.pitch = s.pitch;
                    s.source.loop = s.loop;
                }
            }
        }
        else
        {
            // Merge sounds from this AudioManager into the persistent instance
            MergeSoundsIntoInstance();
            Destroy(gameObject);
            return;
        }
    }

    // Merge sounds from this AudioManager into the persistent instance
    void MergeSoundsIntoInstance()
    {
        if (instance == null || sounds == null || sounds.Length == 0)
            return;

        // Create a list to hold all sounds
        System.Collections.Generic.List<Sound> allSounds = new System.Collections.Generic.List<Sound>();
        
        // Add existing sounds from the persistent instance
        if (instance.sounds != null)
        {
            foreach (Sound s in instance.sounds)
            {
                if (s != null)
                {
                    allSounds.Add(s);
                }
            }
        }

        // Add new sounds from this AudioManager that don't already exist
        foreach (Sound newSound in sounds)
        {
            if (newSound != null && newSound.clip != null)
            {
                // Check if this sound already exists
                bool exists = false;
                foreach (Sound existingSound in allSounds)
                {
                    if (existingSound != null && existingSound.name == newSound.name)
                    {
                        exists = true;
                        break;
                    }
                }

                // If it doesn't exist, add it and create an AudioSource
                if (!exists)
                {
                    allSounds.Add(newSound);
                    newSound.source = instance.gameObject.AddComponent<AudioSource>();
                    newSound.source.clip = newSound.clip;
                    newSound.source.volume = newSound.volume;
                    newSound.source.pitch = newSound.pitch;
                    newSound.source.loop = newSound.loop;
                }
            }
        }

        // Update the instance's sounds array
        instance.sounds = allSounds.ToArray();
    }

    // START #########################################################
    void Start()
    {
        Play("Theme");
    }

    // PLAY #########################################################
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " has no AudioSource!");
            return;
        }
        s.source.Play();
    }

    // STOP #########################################################
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " has no AudioSource!");
            return;
        }
        s.source.Stop();
    }

    // Helper method to safely get the AudioManager instance
    public static AudioManager GetInstance()
    {
        // First try to use the static instance
        if (instance != null && instance.sounds != null && instance.sounds.Length > 0)
        {
            return instance;
        }
        
        // If instance is null or not initialized, try to find it
        AudioManager found = FindObjectOfType<AudioManager>();
        if (found != null && found.sounds != null && found.sounds.Length > 0)
        {
            // Update instance if it was null
            if (instance == null)
            {
                instance = found;
            }
            return found;
        }
        
        return null;
    }
}
