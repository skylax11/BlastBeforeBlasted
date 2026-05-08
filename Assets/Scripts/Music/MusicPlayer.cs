using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource _musicSource;

    [SerializeField]
    private List<AudioClip> _musics;

    public bool EnableMusic;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        StartCoroutine(PlayMusic());
    }
    private IEnumerator PlayMusic()
    {
        while(true && EnableMusic)
        {
            AudioClip randomMusic = _musics[UnityEngine.Random.Range(0, _musics.Count)];
            _musicSource.clip = randomMusic;
            _musicSource.Play();

            yield return new WaitWhile(() => _musicSource.isPlaying);

            yield return new WaitForSeconds(1);
        }
    }
    public void ToggleMusic()
    {
        EnableMusic = !EnableMusic;

        if (EnableMusic)
        {
            StartCoroutine(PlayMusic());
        }
        else
        {
            _musicSource.Stop();
            StopAllCoroutines();
        }
    }
    public void HandleSoundLevel(float soundLevel)
    {
        _musicSource.volume = soundLevel;
    }
}
