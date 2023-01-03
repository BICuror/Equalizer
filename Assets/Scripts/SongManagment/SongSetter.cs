using UnityEngine;

public sealed class SongSetter : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClips;

    [SerializeField] private MusicManager _musicManager;
    private int _currentClipIndex;

    public void SetNextClip()
    {
        _currentClipIndex++;
        if (_currentClipIndex >= _audioClips.Length) _currentClipIndex = 0;

        _musicManager.Play(_audioClips[_currentClipIndex]);
    }

    public void SetPreviousClip()
    {
        _currentClipIndex--;
        if (_currentClipIndex < 0) _currentClipIndex = _audioClips.Length - 1;

        _musicManager.Play(_audioClips[_currentClipIndex]);
    }
}
