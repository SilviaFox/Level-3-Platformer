using UnityEngine;


[System.Serializable]
public class Sound // Creates a class that the AudioManager can call
{

    // Info about an audio clip
    public string name; 

    public AudioClip clip;

    [Range(0f, 3f)]
    public float volume;

    public bool randomPitch;

    [Range(.1f, 3f)]
    public float minPitch;
    [Range(.1f, 3f)]
    public float maxPitch; // This is used if random pitch is unchecked

    public bool loop;

    [HideInInspector]
    public AudioSource source;

}
