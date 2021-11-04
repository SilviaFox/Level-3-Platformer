using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMusicManager : MonoBehaviour
{

    public static GlobalMusicManager instance;

    [SerializeField] AudioClip music;
    [SerializeField] [Range(0,1)] float startVolume = 1;
    [SerializeField] bool playOnStart;
    AudioSource source;

    // Start is called before the first frame update
    void OnEnable()
    {
        instance = this;

        source = this.gameObject.AddComponent<AudioSource>();
        source.clip = music;
        source.loop = true;
        source.volume = startVolume;

        if (playOnStart)
            source.Play();
        
    }

    public void StartMusic()
    {
        source.volume = startVolume;
        source.Play();
    }

    // Update is called once per frame
    public IEnumerator FadeOut(float fadeTime)
    {
        while (source.volume != 0)
        {
            source.volume -= (fadeTime * Time.deltaTime);
            if (source.volume < 0)
                source.volume = 0;
            yield return new WaitForEndOfFrame();
        }

        source.Stop();

    }

    public IEnumerator FadeIn(float fadeTime)
    {
        source.Play();

        while (source.volume != 1)
        {
            source.volume += (fadeTime * Time.deltaTime);
            if (source.volume > 1)
                source.volume = 1;
            yield return new WaitForEndOfFrame();
        }


    }
}
