using UnityEngine;

public sealed class SpectrumDataManager : MonoBehaviour
{
    [Header("SpectrumSettings")]
    [SerializeField] private FFTWindow _FFTAlgorithm;
    [Range(0, 8192)] [SerializeField] private int _sampleRange;

    private float[] _samples;
    private float [] _outputData;

    [Header("Links")]
    [SerializeField] private MusicManager _musicManager;

    private void Awake() => ClearData();
    
    public void ClearData()
    {
        _samples = new float[_sampleRange];

        _outputData = new float[_sampleRange];
    }

    public void UpdateAudioData()
    {
        AudioSource currentSourse = _musicManager.GetMusicSourse();

        currentSourse.GetSpectrumData(_samples, 0, _FFTAlgorithm);   
        
        currentSourse.GetOutputData(_outputData, 0);
    }

    public int GetSampleRange() => _sampleRange;

    public float[] GetSamples() => _samples;

    public float[] GetOutputData() => _outputData;    
}
