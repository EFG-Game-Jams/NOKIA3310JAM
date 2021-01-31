using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    private Dictionary<string, AudioClip> cache;

    private void Awake()
    {
        // load all effects into cache
        var clips = Resources.LoadAll<AudioClip>("AudioClips");
        cache = new Dictionary<string, AudioClip>();
        for (int i = 0; i < clips.Length; ++i)
            cache.Add(clips[i].name, clips[i]);
    }

    public bool Play(string name)
    {
        if (audioSource.isPlaying)
            return false;

        AudioClip clip = cache[name];
        audioSource.clip = clip;
        audioSource.Play();
        return true;
    }
}
