using System.Collections.Generic;
using UnityEngine;

public class AudioManger : MonoBehaviour {
    
    private Dictionary<string, AudioClip> _soundDictionary;

    private AudioSource[] audioSources;

    private AudioSource bgAudioSource;
    private AudioSource audioSourceEffect;
    
    void Awake()
    {
        _soundDictionary = new Dictionary<string, AudioClip>();
        //本地加载 
        AudioClip[] audioArray = Resources.LoadAll<AudioClip>("Audios");

        audioSources = GetComponents<AudioSource>();
        bgAudioSource = audioSources[0];
        audioSourceEffect = audioSources[1];

        //存放到字典
        foreach (AudioClip item in audioArray)
        {
            _soundDictionary.Add(item.name, item);
        }
    }

    public AudioClip GetSoundClip(string audioName) {

        if (_soundDictionary.ContainsKey(audioName))
        {
            return _soundDictionary[audioName];
        }
        return null;
    }

    public void PlayBGAudio(string audioName)
    {
        if (_soundDictionary.ContainsKey(audioName))
        {
            bgAudioSource.clip = _soundDictionary[audioName];
            if (Game.Instance.VoiceOn == 1)
            {
                bgAudioSource.Play();
            }
        }
    }
    public void PlayAudioEffect(string audioEffectName)
    {
        if (_soundDictionary.ContainsKey(audioEffectName))
        {
            if (Game.Instance.VoiceOn == 1)
            {
                AudioSource.PlayClipAtPoint(_soundDictionary[audioEffectName], Vector3.zero);
            }

            //audioSourceEffect.clip = _soundDictionary[audioEffectName];
            //audioSourceEffect.Play();
        }
    }
}
