using UnityEngine;

public sealed class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;

    public AudioSource GetMusicSourse() => _musicSource;

    public void Play(AudioClip clip)
    {   
        _musicSource.clip = clip;

        _musicSource.Play();
    }
}
