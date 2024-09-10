using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    private AudioSource sFXPlayer;
    [SerializeField]
    private AudioSource musicPlayer;

    [SerializeField]
    private float minPitch = 0.9f;
    [SerializeField]
    private float maxPitch = 1.1f;

    [SerializeField]
    private AudioClip forestBgm;
    [SerializeField]
    private AudioClip felkoBgm;


    protected override void Awake()
    {
        base.Awake();
    }

    public void PlaySFX(AudioData audioData)
    {
        sFXPlayer.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    public void PlayRandomSFX(AudioData audioData)
    {
        sFXPlayer.pitch = Random.Range(minPitch, maxPitch);
        PlaySFX(audioData);
    }

    public void PlayRandomSFX(AudioData[] audioDatas)
    {
        PlaySFX(audioDatas[Random.Range(0, audioDatas.Length)]);
    }

    public void FelkoBgmStart()
    {
        musicPlayer.clip = felkoBgm;
        musicPlayer.Play();
    }

    public void ForestBgmStart()
    {
        musicPlayer.clip = forestBgm;
        musicPlayer.Play();
    }
}


[System.Serializable]
public class AudioData
{
    public AudioClip audioClip;
    public float volume;
}
