using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Audio;

public class ObjectAudioManager : MonoBehaviour
{

    public Sound[] sounds; // Call the Sound class created in Sound.cs

    // Calls before start
    void Start()
    {   

        foreach (Sound s in sounds) // For every sound created with the Sound class
        {
            s.source = gameObject.AddComponent<AudioSource>(); // create and assign audio source
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.maxPitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = 0.5f;
        }
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); // find the sound in the array
        if (s == null) // stops errors from occuring from typos
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.randomPitch == true) // If sound has a random pitch
            s.source.pitch = UnityEngine.Random.Range(s.minPitch, s.maxPitch); // set a random pitch

        s.source.Play(); // Play it!
    }

    public void Stop (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); // find sound in the array
        if (s == null) // stops errors from occuring from typos
        {
            Debug.LogWarning("Sound: " + name + "not found!");
            return;
        }
        s.source.Stop(); // Stop it!
    }
    
}
